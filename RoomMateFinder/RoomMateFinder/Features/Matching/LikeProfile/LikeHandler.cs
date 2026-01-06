using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.LikeProfile.LikeRequest;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public class LikeHandler : IRequestHandler<LikeCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly IValidator<LikeRequest> _validator;

    public LikeHandler(AppDbContext db, IValidator<LikeRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<bool> Handle(LikeCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request.Request, cancellationToken);

        var likerUserId = request.UserId;
        var targetProfileId = request.Request.TargetProfileId;

        var liker = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == likerUserId, cancellationToken);

        var targetProfile = await _db.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == targetProfileId, cancellationToken);

        if (liker == null || targetProfile == null || liker.Profile == null)
            return false;

        var targetUser = targetProfile.User;

        // 1️⃣ Creează / actualizează LIKE
        var existingLike = await _db.Likes.FirstOrDefaultAsync(
            x => x.LikerUserId == likerUserId &&
                 x.TargetProfileId == targetProfileId,
            cancellationToken);

        if (existingLike == null)
        {
            _db.Likes.Add(new Like
            {
                Id = Guid.NewGuid(),
                LikerUserId = likerUserId,
                TargetProfileId = targetProfileId,
                IsLike = true,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existingLike.IsLike = true;
            existingLike.CreatedAt = DateTime.UtcNow;
        }

        // 2️⃣ Verifică dacă ESTE MATCH
        var isMatch = await _db.Likes.AnyAsync(
            l => l.IsLike &&
                 l.LikerUserId == targetUser.Id &&
                 l.TargetProfileId == liker.Profile.Id,
            cancellationToken);

        // 3️⃣ Creează CONVERSATION doar dacă este MATCH
        if (isMatch)
        {
            var userA = likerUserId;
            var userB = targetUser.Id;

            // ordine deterministă → evită duplicate
            var first = userA.CompareTo(userB) < 0 ? userA : userB;
            var second = userA.CompareTo(userB) < 0 ? userB : userA;

            var conversationExists = await _db.Conversations.AnyAsync(
                c => c.User1Id == first && c.User2Id == second,
                cancellationToken);

            if (!conversationExists)
            {
                _db.Conversations.Add(new Conversation
                {
                    Id = Guid.NewGuid(),
                    User1Id = first,
                    User2Id = second,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
