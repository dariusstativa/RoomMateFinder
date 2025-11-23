using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public class DislikeValidator : AbstractValidator<DislikeRequest>
{
    public DislikeValidator(AppDbContext db)
    {
        RuleFor(x => x.LikerUserId)
            .MustAsync(async (id, ct) => 
                await db.Users.AnyAsync(u => u.Id == id, ct))
            .WithMessage("User not found.");

        RuleFor(x => x.TargetProfileId)
            .MustAsync(async (id, ct) =>
                await db.Profiles.AnyAsync(p => p.Id == id, ct))
            .WithMessage("Target profile not found.");

        RuleFor(x => x)
            .Must(x => x.LikerUserId != x.TargetProfileId)
            .WithMessage("You cannot dislike your own profile.");
    }
}