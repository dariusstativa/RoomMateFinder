using System.Security.Claims;
using MediatR;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetMyProfileEndpoint
{
    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/me", async (ClaimsPrincipal user, IMediator mediator, CancellationToken ct) =>
            {
                var idStr = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(idStr, out var userId))
                    return Results.Unauthorized();

                var profile = await mediator.Send(new GetMyProfileQuery(userId), ct);
                return profile is not null ? Results.Ok(profile) : Results.NotFound();
            })
            .RequireAuthorization();
    }
}