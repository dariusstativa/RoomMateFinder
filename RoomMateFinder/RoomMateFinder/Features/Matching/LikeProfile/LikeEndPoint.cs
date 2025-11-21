using MediatR;
using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public static class LikeEndpoint
{
    public static void MapLikeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/like", async (LikeRequest req, IMediator mediator) =>
        {
            bool ok = await mediator.Send(new LikeCommand(req));
            return ok ? Results.Ok() : Results.BadRequest();
        });
    }
}