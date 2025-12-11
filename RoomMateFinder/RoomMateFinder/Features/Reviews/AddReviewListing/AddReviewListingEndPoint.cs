using System.Security.Claims;
using MediatR;

namespace RoomMateFinder.Features.Reviews.AddReviewListing;

public static class AddReviewForListingEndpoint
{
    public static void MapAddReviewForListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/listings/{id:guid}/reviews",
                async (Guid id, AddReviewForListingRequest body, HttpContext ctx, IMediator mediator) =>
                {
                    var reviewerId = Guid.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                    var cmd = new AddReviewForListingCommand(
                        ListingId: id,
                        ReviewerId: reviewerId,
                        Rating: body.Rating,
                        Comment: body.Comment
                    );

                    var result = await mediator.Send(cmd);
                    return Results.Ok(result);
                })
            .RequireAuthorization();
    }
}
