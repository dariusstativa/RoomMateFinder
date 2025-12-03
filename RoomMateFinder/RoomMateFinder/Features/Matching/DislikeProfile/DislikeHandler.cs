<<<<<<< HEAD
﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Matching.Rating;
=======
﻿using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
>>>>>>> DariusBranch
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public class DislikeHandler : IRequestHandler<DislikeCommand, bool>
{
    private readonly AppDbContext _db;
<<<<<<< HEAD

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
=======
    private readonly IValidator<DislikeRequest> _validator;

    public DislikeHandler(AppDbContext db, IValidator<DislikeRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<bool> Handle(DislikeCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request.Request, cancellationToken);

        var userId = request.UserId;
        var targetId = request.Request.TargetProfileId;

        var existing = await _db.Likes
            .FirstOrDefaultAsync(
                x => x.LikerUserId == userId && x.TargetProfileId == targetId,
                cancellationToken);

        if (existing != null)
        {
            existing.IsLike = false;
            existing.CreatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
>>>>>>> DariusBranch

        var dislike = new Like
        {
            Id = Guid.NewGuid(),
<<<<<<< HEAD
            LikerUserId = disliker.Id,
            TargetProfileId = targetProfile.Id,
=======
            LikerUserId = userId,
            TargetProfileId = targetId,
>>>>>>> DariusBranch
            IsLike = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(dislike);
<<<<<<< HEAD

        targetUser.Rating = EloCalculator.CalculateNewRating(
            targetUser.Rating,
            disliker.Rating,
            isWin: false
        );

        await _db.SaveChangesAsync(ct);
=======
        await _db.SaveChangesAsync(cancellationToken);

>>>>>>> DariusBranch
        return true;
    }
}