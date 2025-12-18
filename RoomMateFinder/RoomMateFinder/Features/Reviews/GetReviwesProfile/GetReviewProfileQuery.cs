using MediatR;

namespace RoomMateFinder.Features.Reviews.GetReviwesProfile;

public record GetReviewsForProfileQuery(Guid ProfileId)
    : IRequest<List<ReviewDto>>;