using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.RoomListings.CreateListing;

public static class CreateListingEndpoint
{
    public static void MapCreateListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/listings", async (RoomListing listing, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateListingCommand(listing));
            return Results.Created($"/listings/{id}", id);
        });
    }
}