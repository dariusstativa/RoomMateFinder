using FluentValidation;

namespace RoomMateFinder.Features.Reviews.AddReviewListing;

public class AddReviewForListingValidator : AbstractValidator<AddReviewForListingCommand>
{
    public AddReviewForListingValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(500);
    }
}