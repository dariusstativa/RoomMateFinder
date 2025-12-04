using MediatR;

namespace RoomMateFinder.Features.Matching.GetRecommendations;

public static class GetRecommendationsEndpoint
{
    public static void MapRecommendationsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/recommendations/{userId:guid}", async (
            Guid userId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetRecommendationsQuery(userId), ct);
            return Results.Ok(result);
        });
    }
}