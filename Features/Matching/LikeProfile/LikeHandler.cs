using FluentValidation;
using FluentValidation.Results;
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

        var existing = await _db.Likes.FirstOrDefaultAsync(
            x => x.LikerUserId == likerUserId && x.TargetProfileId == targetProfileId,
            cancellationToken
        );

        if (existing != null)
        {
            existing.IsLike = true;
            existing.CreatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        var like = new Like
        {
            Id = Guid.NewGuid(),
            LikerUserId = likerUserId,
            TargetProfileId = targetProfileId,
            IsLike = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(like);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}