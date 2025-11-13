using MediatR;

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

public static class DeleteListingEndpoint
{
    public static void MapDeleteListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/listings/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var ok = await mediator.Send(new DeleteListingCommand(id));
            return ok ? Results.NoContent() : Results.NotFound();
        });
    }
}