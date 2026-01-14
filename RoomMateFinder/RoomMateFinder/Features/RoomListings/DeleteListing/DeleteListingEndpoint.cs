using MediatR;
using System.Security.Claims;

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

public static class SDeleteListingEndpoint
{
    public static void MapDeleteListingEndpoint(this IEndpointRouteBuilder app)
    {
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
    }
}