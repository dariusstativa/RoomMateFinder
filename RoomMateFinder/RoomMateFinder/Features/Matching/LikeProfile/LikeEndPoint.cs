using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

using MediatR;
using Microsoft.AspNetCore.Mvc;


public static class LikeEndpoint
{
    public static void MapLikeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/like", async (
            [FromBody] LikeRequest req,
            IMediator mediator) =>
        {
            var ok = await mediator.Send(new LikeCommand(req));
            return ok ? Results.Ok("Liked") : Results.BadRequest();
        });
    }
}
