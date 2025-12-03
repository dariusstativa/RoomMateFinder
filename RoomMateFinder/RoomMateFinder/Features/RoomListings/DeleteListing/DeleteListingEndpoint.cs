using MediatR;
<<<<<<< HEAD
=======
using System.Security.Claims;
>>>>>>> DariusBranch

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

public static class DeleteListingEndpoint
{
    public static void MapDeleteListingEndpoint(this IEndpointRouteBuilder app)
    {
<<<<<<< HEAD
        app.MapDelete("/listings/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var ok = await mediator.Send(new DeleteListingCommand(id));
            return ok ? Results.NoContent() : Results.NotFound();
        });
=======
        app.MapDelete("/listings/{id:guid}", async (
                HttpContext http,
                Guid id,
                IMediator mediator) =>
            {
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var ok = await mediator.Send(new DeleteListingCommand(id, userId));

                return ok ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization();
>>>>>>> DariusBranch
    }
}