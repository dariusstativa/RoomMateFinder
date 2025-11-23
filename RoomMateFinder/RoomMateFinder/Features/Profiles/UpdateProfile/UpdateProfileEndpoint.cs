using MediatR;
using System.Security.Claims;

namespace RoomMateFinder.Features.Profiles.UpdateProfile;

public static class UpdateProfileEndpoint
{
    public static void MapUpdateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/profiles/{id:guid}", async (HttpContext http, Guid id, UpdateProfileRequest req, IMediator mediator) =>
            {
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                
                // Verifică dacă userId din token corespunde cu id din URL
                if (userId != id)
                {
                    return Results.Forbid();
                }

                var success = await mediator.Send(new UpdateProfileCommand(userId, req));

                return success ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization();
    }
}