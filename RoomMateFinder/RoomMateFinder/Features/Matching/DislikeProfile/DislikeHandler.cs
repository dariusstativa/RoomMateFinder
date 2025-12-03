using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Matching.Rating;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public class DislikeHandler : IRequestHandler<DislikeCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly IValidator<DislikeRequest> _validator;

    public DislikeHandler(AppDbContext db, IValidator<DislikeRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<bool> Handle(DislikeCommand request, CancellationToken cancellationToken)
    {
        // ---------------------------
        // VALIDATE REQUEST
        // ---------------------------
        await _validator.ValidateAndThrowAsync(request.Request, cancellationToken);

        var userId = request.UserId;
        var targetId = request.Request.TargetProfileId;

        // ---------------------------
        // LOAD DISLIKER + TARGET
        // ---------------------------
        var disliker = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var targetProfile = await _db.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == targetId, cancellationToken);

        if (disliker == null || targetProfile == null)
            return false;

        var targetUser = targetProfile.User;

        // ---------------------------
        // CHECK IF LIKE EXISTS
        // ---------------------------
        var existing = await _db.Likes
            .FirstOrDefaultAsync(
                x => x.LikerUserId == userId && x.TargetProfileId == targetId,
                cancellationToken);

        if (existing != null)
        {
            // Convert like ➜ dislike
            existing.IsLike = false;
            existing.CreatedAt = DateTime.UtcNow;

            // Update target rating (HEAD logic preserved)
            targetUser.Rating = EloCalculator.CalculateNewRating(
                targetUser.Rating,
                disliker.Rating,
                isWin: false);

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        // ---------------------------
        // CREATE NEW DISLIKE
        // ---------------------------
        var dislike = new Like
        {
            Id = Guid.NewGuid(),
            LikerUserId = userId,
            TargetProfileId = targetId,
            IsLike = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(dislike);

        // Update target rating (HEAD logic preserved)
        targetUser.Rating = EloCalculator.CalculateNewRating(
            targetUser.Rating,
            disliker.Rating,
            isWin: false);

        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
