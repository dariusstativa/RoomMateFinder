using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.LikeProfile.LikeRequest;
using RoomMateFinder.Features.Matching.Rating;
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
        // Validate incoming request
        await _validator.ValidateAndThrowAsync(request.Request, cancellationToken);

        var likerUserId = request.UserId;
        var targetProfileId = request.Request.TargetProfileId;

        // Load liker + profile + rating
        var liker = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == likerUserId, cancellationToken);

        // Load target profile + linked user
        var targetProfile = await _db.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == targetProfileId, cancellationToken);

        if (liker == null || targetProfile == null)
            return false;

        var targetUser = targetProfile.User;

        // Check existing like/dislike
        var existing = await _db.Likes.FirstOrDefaultAsync(
            x => x.LikerUserId == likerUserId && x.TargetProfileId == targetProfileId,
            cancellationToken);

        if (existing != null)
        {
            existing.IsLike = true;
            existing.CreatedAt = DateTime.UtcNow;

            // Apply ELO rating logic (kept from HEAD)
            targetUser.Rating = EloCalculator.CalculateNewRating(
                targetUser.Rating,
                liker.Rating,
                isWin: true);

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Create new like
        var like = new Like
        {
            Id = Guid.NewGuid(),
            LikerUserId = likerUserId,
            TargetProfileId = targetProfileId,
            IsLike = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(like);

        // Apply rating update
        targetUser.Rating = EloCalculator.CalculateNewRating(
            targetUser.Rating,
            liker.Rating,
            isWin: true);

        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
