using System.Security.Claims;
using MediatR;


namespace RoomMateFinder.Features.Profiles.GetAllProfiles;

public static class GetAllProfilesEndpoint
{
    public static void MapGetAllProfilesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles", async (
                HttpContext http,
                IMediator mediator) =>
            {
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim.Value);

                var profiles = await mediator.Send(
                    new GetAllProfilesQuery(userId));

                return Results.Ok(profiles);
            })
            .RequireAuthorization();

    }
}