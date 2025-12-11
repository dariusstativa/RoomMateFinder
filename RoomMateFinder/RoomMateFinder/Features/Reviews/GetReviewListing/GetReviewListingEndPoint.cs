using MediatR;

namespace RoomMateFinder.Features.Reviews.GetReviewListing;

public static class GetReviewsForListingEndpoint
{
    public static void MapGetReviewsForListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/listings/{id:guid}/reviews",
                async (Guid id, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetReviewsForListingQuery(id));
                    return Results.Ok(result);
                })
            .RequireAuthorization();
    }
}