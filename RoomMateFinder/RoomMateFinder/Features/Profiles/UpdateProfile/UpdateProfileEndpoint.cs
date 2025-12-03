using MediatR;
<<<<<<< HEAD
=======
using System.Security.Claims;
>>>>>>> DariusBranch

namespace RoomMateFinder.Features.Profiles.UpdateProfile;

public static class UpdateProfileEndpoint
{
    public static void MapUpdateProfileEndpoint(this IEndpointRouteBuilder app)
    {
<<<<<<< HEAD
        app.MapPut("/profiles/{userId:guid}", async (Guid userId, UpdateProfileRequest req, IMediator mediator) =>
        {
            var success = await mediator.Send(new UpdateProfileCommand(userId, req));
            return success ? Results.NoContent() : Results.NotFound();
        });
    }
}
=======
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
>>>>>>> DariusBranch
