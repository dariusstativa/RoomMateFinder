using MediatR;
using FluentValidation;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public static class DislikeEndpoint
{
    public static void MapDislikeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/dislike", async (
            DislikeRequest request,
            IMediator mediator,
            IValidator<DislikeRequest> validator) =>
        {
            await validator.ValidateAndThrowAsync(request);

            var result = await mediator.Send(new DislikeCommand(request));
            return Results.Ok(new { success = result });
        });
    }
}