using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
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

        var dislike = new Like
        {
            Id = Guid.NewGuid(),
            LikerUserId = userId,
            TargetProfileId = targetId,
            IsLike = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(dislike);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}