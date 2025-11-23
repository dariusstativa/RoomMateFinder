using MediatR;
<<<<<<< HEAD
using System.Security.Claims;
=======
>>>>>>> CleanFixBranch

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
<<<<<<< HEAD
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
=======
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);
>>>>>>> CleanFixBranch

                var matches = await mediator.Send(new GetMatchesQuery(userId), ct);

                return matches.Count > 0
                    ? Results.Ok(matches)
                    : Results.NotFound("No matches found.");
            })
            .RequireAuthorization();
    }
}