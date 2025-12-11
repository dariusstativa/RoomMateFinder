using MediatR;

namespace RoomMateFinder.Features.Reviews.AddReviewProfile;

public record AddReviewForProfileCommand(
    Guid ProfileId,
    Guid ReviewerId,
    int Rating,
    string Comment
) : IRequest<Guid>;