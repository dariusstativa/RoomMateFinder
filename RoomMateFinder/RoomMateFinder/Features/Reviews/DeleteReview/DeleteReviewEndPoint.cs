using MediatR;

namespace RoomMateFinder.Features.Reviews.DeleteReview;

public static class DeleteReviewEndpoint
{
    public static void MapDeleteReviewEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/reviews/{id:guid}",
                async (Guid id, IMediator mediator) =>
                {
                    var result = await mediator.Send(new DeleteReviewCommand(id));
                    return result ? Results.Ok() : Results.NotFound();
                })
            .RequireAuthorization();
    }
}