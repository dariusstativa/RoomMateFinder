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
    {_validator = validator;
        _db = db;
    }

    public async Task<bool> Handle(DislikeCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = _validator.Validate(request.Request);
        if (!validationResult.IsValid)
        {
            throw new  ValidationException(validationResult.Errors);
        }
        var req = request.Request;

        var existing = await _db.Likes
            .FirstOrDefaultAsync(x =>
                    x.LikerUserId == req.LikerUserId &&
                    x.TargetProfileId == req.TargetProfileId,
                cancellationToken);

        if (existing != null)
        {
            existing.IsLike = false;   
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        var dislike = new Like
        {
            LikerUserId = req.LikerUserId,
            TargetProfileId = req.TargetProfileId,
            IsLike = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(dislike);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}