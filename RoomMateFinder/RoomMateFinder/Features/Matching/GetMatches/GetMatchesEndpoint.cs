using MediatR;

namespace RoomMateFinder.Features.Matching.GetMatches;

public static class GetMatchesEndpoint
{
    public static void MapGetMatchesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matches/{userId:guid}", async (
            Guid userId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var matches = await mediator.Send(new GetMatchesQuery(userId), ct);

            return matches.Count > 0
                ? Results.Ok(matches)
                : Results.NotFound("No matches found.");
        });
    }
}