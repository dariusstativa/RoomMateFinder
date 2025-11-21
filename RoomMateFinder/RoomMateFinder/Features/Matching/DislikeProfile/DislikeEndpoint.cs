using MediatR;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public static class DislikeEndpoint
{
    public static void MapDislikeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/dislike", async (Guid likerId, Guid targetId, IMediator mediator) =>
        {
            bool ok = await mediator.Send(new DislikeCommand(likerId, targetId));
            return ok ? Results.Ok() : Results.BadRequest();
        });
    }
}