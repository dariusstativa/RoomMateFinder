using FluentValidation;
using RoomMateFinder.Features.Profiles.UpdateProfile;

namespace RoomMateFinder.Features.Profiles.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileValidator()
    {
      
        RuleFor(x => x.SleepSchedule)
            .NotEmpty().WithMessage("SleepSchedule is required.");

        RuleFor(x => x.Cleanliness)
            .NotEmpty().WithMessage("Cleanliness is required.");

        RuleFor(x => x.NoiseTolerance)
            .NotEmpty().WithMessage("NoiseTolerance is required.");

        RuleFor(x => x.SmokingPreference)
            .NotEmpty().WithMessage("SmokingPreference is required.");

        RuleFor(x => x.PetPreference)
            .NotEmpty().WithMessage("PetPreference is required.");

        RuleFor(x => x.StudyHabits)
            .NotEmpty().WithMessage("StudyHabits is required.");
    }
}