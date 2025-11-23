using MediatR;
<<<<<<< HEAD
using System.Security.Claims;
=======
>>>>>>> CleanFixBranch

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public static class UpdateListingEndpoint
{
    public static void MapUpdateListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/listings/{id:guid}", async (
                HttpContext http,
                Guid id,
                UpdateListingRequest req,
                IMediator mediator) =>
            {
<<<<<<< HEAD
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
=======
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);
>>>>>>> CleanFixBranch

                var ok = await mediator.Send(new UpdateListingCommand(id, userId, req));

                return ok ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization();
    }
}