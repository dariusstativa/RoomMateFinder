using MediatR;

namespace RoomMateFinder.Features.Reviews.GetReviwesProfile;

public static class GetReviewsForProfileEndpoint
{
    public static void MapGetReviewsForProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/{id:guid}/reviews",
                async (Guid id, IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetReviewsForProfileQuery(id));
                    return Results.Ok(result);
                })
            .RequireAuthorization();
    }
}