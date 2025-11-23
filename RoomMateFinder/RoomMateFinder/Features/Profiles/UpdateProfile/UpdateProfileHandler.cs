using FluentValidation;
using FluentValidation.Results;
using MediatR;
using RoomMateFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Features.Profiles.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, UpdateProfileRequest Request) : IRequest<bool>;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IValidator<UpdateProfileRequest> _validator;

    public UpdateProfileHandler(AppDbContext context, IValidator<UpdateProfileRequest> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<bool> Handle(UpdateProfileCommand cmd, CancellationToken ct)
    {
        ValidationResult validationResult=_validator.Validate(cmd.Request);
        if (!validationResult.IsValid)
        {
            throw  new ValidationException(validationResult.Errors);
        }
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);
        if (profile == null) return false;

      
        profile.Bio = cmd.Request.Bio ?? profile.Bio;
        profile.SleepSchedule = cmd.Request.SleepSchedule ?? profile.SleepSchedule;
        profile.Cleanliness = cmd.Request.Cleanliness ?? profile.Cleanliness;
        profile.NoiseTolerance = cmd.Request.NoiseTolerance ?? profile.NoiseTolerance;
        profile.SmokingPreference = cmd.Request.SmokingPreference ?? profile.SmokingPreference;
        profile.PetPreference = cmd.Request.PetPreference ?? profile.PetPreference;
        profile.StudyHabits = cmd.Request.StudyHabits ?? profile.StudyHabits;

        await _context.SaveChangesAsync();
        return true;
    }
}