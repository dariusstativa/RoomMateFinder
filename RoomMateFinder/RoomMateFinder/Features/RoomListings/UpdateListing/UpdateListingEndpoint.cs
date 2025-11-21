using MediatR;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public static class UpdateListingEndpoint
{
    public static void MapUpdateListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/listings/{id:guid}", async (Guid id, UpdateListingRequest req, IMediator mediator) =>
        {
            var ok = await mediator.Send(new UpdateListingCommand(id, req));
            return ok ? Results.NoContent() : Results.NotFound();
        });
    }
}