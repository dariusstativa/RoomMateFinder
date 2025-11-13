using FluentValidation;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public class UpdateListingValidator : AbstractValidator<UpdateListingRequest>
{
    public UpdateListingValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.RoommatesCount)
            .InclusiveBetween(0,4).WithMessage("Roommates count must be reasonable.");

        RuleFor(x => x.GenderPreference)
            .NotEmpty().WithMessage("Gender preference cannot be empty.");
    }
}