using MediatR;
using System.Security.Claims;

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
                // Încearcă mai întâi ClaimTypes.NameIdentifier (frontend coleg)
                // Dacă nu există, folosește fallback "sub" (standard JWT)
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim.Value);

                var matches = await mediator.Send(new GetMatchesQuery(userId), ct);

                return matches.Count > 0
                    ? Results.Ok(matches)
                    : Results.NotFound("No matches found.");
            })
            .RequireAuthorization();
    }
}