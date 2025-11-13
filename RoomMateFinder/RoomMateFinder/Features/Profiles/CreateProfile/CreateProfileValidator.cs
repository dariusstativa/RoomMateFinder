using FluentValidation;

namespace RoomMateFinder.Features.Profiles.CreateProfile;

public class CreateProfileValidator : AbstractValidator<CreateProfileRequest>
{
    public CreateProfileValidator()
    {
        RuleFor(p => p.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100).WithMessage("Full Name must not exceed 100 characters.");

        RuleFor(p => p.Age)
            .NotEmpty().WithMessage("Age is required.")
            .InclusiveBetween(18, 99)
            .WithMessage("Age must be between 18 and 99.");

        RuleFor(p => p.Gender)
            .NotEmpty().WithMessage("Gender is required.");

        RuleFor(p => p.University)
            .NotEmpty().WithMessage("University is required.");

        
        RuleFor(p => p.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.")
            .When(p => !string.IsNullOrWhiteSpace(p.Bio));

        RuleFor(p => p.SleepSchedule)
            .NotEmpty().WithMessage("SleepSchedule is required.");

        RuleFor(p => p.Cleanliness)
            .NotEmpty().WithMessage("Cleanliness is required.");

        RuleFor(p => p.NoiseTolerance)
            .NotEmpty().WithMessage("NoiseTolerance is required.");

        RuleFor(p => p.SmokingPreference)
            .NotEmpty().WithMessage("SmokingPreference is required.");

        RuleFor(p => p.PetPreference)
            .NotEmpty().WithMessage("PetPreference is required.");

        RuleFor(p => p.StudyHabits)
            .NotEmpty().WithMessage("StudyHabits is required.");
    }
}