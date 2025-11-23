using MediatR;
<<<<<<< HEAD
using System.Security.Claims;
=======
>>>>>>> CleanFixBranch

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

public static class DeleteListingEndpoint
{
    public static void MapDeleteListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/listings/{id:guid}", async (
                HttpContext http,
                Guid id,
                IMediator mediator) =>
            {
<<<<<<< HEAD
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
=======
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);
>>>>>>> CleanFixBranch

                var ok = await mediator.Send(new DeleteListingCommand(id, userId));

                return ok ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization();
    }
}