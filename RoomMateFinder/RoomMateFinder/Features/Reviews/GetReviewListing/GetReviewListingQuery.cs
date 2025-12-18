using MediatR;

namespace RoomMateFinder.Features.Reviews.GetReviewListing;

public record GetReviewsForListingQuery(Guid ListingId)
    : IRequest<List<ReviewDto>>;