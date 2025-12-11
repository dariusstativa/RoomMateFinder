using MediatR;

namespace RoomMateFinder.Features.Reviews.AddReviewListing;

public record AddReviewForListingCommand(
    Guid ListingId,
    Guid ReviewerId,
    int Rating,
    string Comment
) : IRequest<Guid>;