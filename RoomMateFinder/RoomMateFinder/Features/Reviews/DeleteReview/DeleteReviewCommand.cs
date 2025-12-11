using MediatR;

namespace RoomMateFinder.Features.Reviews.DeleteReview;

public record DeleteReviewCommand(Guid ReviewId) : IRequest<bool>;