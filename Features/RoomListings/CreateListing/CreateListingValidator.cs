using FluentValidation;

namespace RoomMateFinder.Features.RoomListings.CreateListing;

public class CreateRoomListingValidator : AbstractValidator<CreateListingRequest>
{
    public CreateRoomListingValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.RoommatesCount)
            .InclusiveBetween(0, 4)
            .WithMessage("RoommatesCount must be between 0 and 4.");

        RuleFor(x => x.GenderPreference)
            .Must(g => g is "Male" or "Female" or "Any")
            .WithMessage("GenderPreference must be one of: Male, Female, Any.");
    }
}