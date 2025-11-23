using MediatR;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using System.Security.Claims;
=======
>>>>>>> CleanFixBranch

namespace RoomMateFinder.Features.Profiles.DeleteProfile;

public static class DeleteProfileEndpoint
{
    public static void MapDeleteProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/profiles", async (
                HttpContext http,
                IMediator mediator) =>
            {
               
<<<<<<< HEAD
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
=======
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);
>>>>>>> CleanFixBranch

               
                var result = await mediator.Send(new DeleteProfileCommand(userId));

                return result ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization(); 
    }
}