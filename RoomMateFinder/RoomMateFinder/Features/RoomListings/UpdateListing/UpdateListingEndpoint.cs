using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public static class UpdateListingEndpoint
{
    public static void MapUpdateListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/listings/{id:guid}", async (Guid id, RoomListing updated, IMediator mediator) =>
        {
            var ok = await mediator.Send(new UpdateListingCommand(id, updated));
            return ok ? Results.NoContent() : Results.NotFound();
        });
    }
}