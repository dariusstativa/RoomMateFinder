using FluentValidation;
using FluentValidation.Results;
using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;


public class LikeHandler : IRequestHandler<LikeCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly IValidator<LikeRequest> _validator;
    public LikeHandler(AppDbContext db, IValidator<LikeRequest> validator)
    {
        _validator = validator;
        _db = db;
    }

    public async Task<bool> Handle(LikeCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = _validator.Validate(request.Request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var req = request.Request;

        
        var existing = await _db.Likes.FirstOrDefaultAsync(
            x => x.LikerUserId == req.LikerUserId && x.TargetProfileId == req.TargetProfileId,
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
            LikerUserId = req.LikerUserId,
            TargetProfileId = req.TargetProfileId,
            IsLike = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Likes.Add(like);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
