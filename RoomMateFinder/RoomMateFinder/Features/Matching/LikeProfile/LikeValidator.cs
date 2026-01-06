using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;



public class LikeValidator : AbstractValidator<LikeRequest>
{
    public LikeValidator(AppDbContext db)
    {
      

        RuleFor(x => x.TargetProfileId)
            .MustAsync(async (id, ct) =>
                await db.Profiles.AnyAsync(p => p.Id == id, ct))
            .WithMessage("Profile not found.");
        
    }
}
