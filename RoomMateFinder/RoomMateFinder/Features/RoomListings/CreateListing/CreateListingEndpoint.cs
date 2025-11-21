using MediatR;
using RoomMateFinder.Features.RoomListings.CreateListing;

public static class CreateRoomListingEndpoint
{
    public static void MapCreateRoomListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/{ownerId:guid}/listings", async (
            Guid ownerId,
            CreateListingRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var id = await mediator.Send(new CreateRoomListingCommand(ownerId, request), ct);
            return Results.Created($"/listings/{id}", id);
        });
    }
}