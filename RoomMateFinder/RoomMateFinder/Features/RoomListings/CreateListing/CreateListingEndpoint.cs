using MediatR;
using RoomMateFinder.Features.RoomListings.CreateListing;
using System.Security.Claims;

public static class CreateRoomListingEndpoint
{
    public static void MapCreateRoomListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/listings", async (
                HttpContext http,
                CreateListingRequest request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var ownerId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var id = await mediator.Send(new CreateRoomListingCommand(ownerId, request), ct);

                return Results.Created($"/listings/{id}", id);
            })
            .RequireAuthorization();
    }
}