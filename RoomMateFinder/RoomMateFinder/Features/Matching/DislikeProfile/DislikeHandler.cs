using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Matching.Rating;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public class DislikeHandler : IRequestHandler<DislikeCommand, bool>
{
    private readonly AppDbContext _db;

    public DislikeHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DislikeCommand request, CancellationToken ct)
    {
        var disliker = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == request.LikerUserId, ct);

        var targetProfile = await _db.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == request.TargetProfileId, ct);

        if (disliker == null || targetProfile == null)
            return false;

        var targetUser = targetProfile.User;

        var dislike = new Like
        {
            Id = Guid.NewGuid(),
            LikerUserId = disliker.Id,
            TargetProfileId = targetProfile.Id,
            IsLike = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(dislike);

        targetUser.Rating = EloCalculator.CalculateNewRating(
            targetUser.Rating,
            disliker.Rating,
            isWin: false
        );

        await _db.SaveChangesAsync(ct);
        return true;
    }
}