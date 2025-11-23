using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RoomMateFinder.Features.Profiles.DeleteProfile;

public static class DeleteProfileEndpoint
{
    public static void MapDeleteProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/profiles", async (
                HttpContext http,
                IMediator mediator) =>
            {
               
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

               
                var result = await mediator.Send(new DeleteProfileCommand(userId));

                return result ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization(); 
    }
}