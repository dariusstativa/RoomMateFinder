using MediatR;

namespace RoomMateFinder.Features.RoomListings.GetAllListings;

public static class GetAllListingsEndpoint
{
    public static void MapGetAllListingsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/listings", async (IMediator mediator) =>
        {
            var list = await mediator.Send(new GetAllListingsQuery());
            return Results.Ok(list);
        });
    }
}