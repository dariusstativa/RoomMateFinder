using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.LikeProfile.LikeRequest;
<<<<<<< HEAD
using RoomMateFinder.Features.Matching.Rating;
=======
>>>>>>> DariusBranch
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

<<<<<<< HEAD
    public async Task<bool> Handle(LikeCommand request, CancellationToken ct)
    {
        ValidationResult validation = _validator.Validate(request.Request);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var req = request.Request;

        var liker = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == req.LikerUserId, ct);

        var targetProfile = await _db.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == req.TargetProfileId, ct);

        if (liker == null || targetProfile == null)
            return false;

        var targetUser = targetProfile.User;

        var existing = await _db.Likes.FirstOrDefaultAsync(
            l => l.LikerUserId == req.LikerUserId && l.TargetProfileId == req.TargetProfileId,
            ct);
=======
    public async Task<bool> Handle(LikeCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request.Request, cancellationToken);

        var likerUserId = request.UserId;
        var targetProfileId = request.Request.TargetProfileId;

        var existing = await _db.Likes.FirstOrDefaultAsync(
            x => x.LikerUserId == likerUserId && x.TargetProfileId == targetProfileId,
            cancellationToken
        );
>>>>>>> DariusBranch

        if (existing != null)
        {
            existing.IsLike = true;
            existing.CreatedAt = DateTime.UtcNow;

<<<<<<< HEAD
            targetUser.Rating = EloCalculator.CalculateNewRating(
                targetUser.Rating,
                liker.Rating,
                isWin: true
            );

            await _db.SaveChangesAsync(ct);
=======
            await _db.SaveChangesAsync(cancellationToken);
>>>>>>> DariusBranch
            return true;
        }

        var like = new Like
        {
            Id = Guid.NewGuid(),
<<<<<<< HEAD
            LikerUserId = req.LikerUserId,
            TargetProfileId = req.TargetProfileId,
=======
            LikerUserId = likerUserId,
            TargetProfileId = targetProfileId,
>>>>>>> DariusBranch
            IsLike = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(like);
<<<<<<< HEAD

        targetUser.Rating = EloCalculator.CalculateNewRating(
            targetUser.Rating,
            liker.Rating,
            isWin: true
        );

        await _db.SaveChangesAsync(ct);
        return true;
    }
}
=======
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
>>>>>>> DariusBranch
