using MediatR;
using System.Security.Claims;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public static class GetProfileEndpoint
{
    public static void MapGetMyProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles/me", async (HttpContext http, IMediator mediator, CancellationToken ct) =>
            {
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var profile = await mediator.Send(new GetProfileQuery(userId), ct);

                return profile is not null
                    ? Results.Ok(profile)
                    : Results.NotFound();
            })
            .RequireAuthorization();
    }
}