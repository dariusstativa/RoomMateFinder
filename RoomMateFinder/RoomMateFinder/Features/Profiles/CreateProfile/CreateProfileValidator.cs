namespace RoomMateFinder.Features.Profiles.CreateProfile;

using FluentValidation;

public class CreateProfileValidator : AbstractValidator<CreateProfileRequest>
{
    public CreateProfileValidator()
    {
        RuleFor(p => p.FullName).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Age).InclusiveBetween(18, 99);
        RuleFor(p => p.University).NotEmpty();
    }
}
