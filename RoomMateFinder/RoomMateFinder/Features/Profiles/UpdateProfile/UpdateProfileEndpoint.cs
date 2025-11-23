using MediatR;
using System.Security.Claims;

namespace RoomMateFinder.Features.Profiles.UpdateProfile;

public static class UpdateProfileEndpoint
{
    public static void MapUpdateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/profiles", async (
                HttpContext http,
                UpdateProfileRequest req,
                IMediator mediator
            ) =>
            {
                // Luăm userId-ul real din token
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var success = await mediator.Send(new UpdateProfileCommand(userId, req));

                return success ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization();
    }
}