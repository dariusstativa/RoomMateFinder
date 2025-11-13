using MediatR;

namespace RoomMateFinder.Features.RoomListings.GetListingById;

public static class GetListingByIdEndpoint
{
    public static void MapGetListingByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/listings/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetListingByIdQuery(id));
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });
    }
}