using MediatR;
using Microsoft.AspNetCore.Routing;
using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public static class LikeEndpoints
{
   
    public static void MapLikeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/like", async (
            LikeRequest req,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var ok = await mediator.Send(new LikeCommand(req), ct);
            return ok ? Results.Ok() : Results.BadRequest();
        });
    }
}