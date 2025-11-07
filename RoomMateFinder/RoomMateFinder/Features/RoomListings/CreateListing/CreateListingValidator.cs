using FluentValidation;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.RoomListings.CreateListing;

public class CreateListingValidator : AbstractValidator<RoomListing>
{
    public CreateListingValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Address).NotEmpty();
    }
}