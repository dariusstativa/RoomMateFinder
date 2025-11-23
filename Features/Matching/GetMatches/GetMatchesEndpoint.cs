using MediatR;

namespace RoomMateFinder.Features.Matching.GetMatches;

public static class GetMatchesEndpoint
{
    public static void MapGetMatchesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matches", async (
                HttpContext http,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);

                var matches = await mediator.Send(new GetMatchesQuery(userId), ct);

                return matches.Count > 0
                    ? Results.Ok(matches)
                    : Results.NotFound("No matches found.");
            })
            .RequireAuthorization();
    }
}