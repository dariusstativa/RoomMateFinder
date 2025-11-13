

using FluentValidation;
using RoomMateFinder.Features.LikeProfile.LikeRequest;


namespace RoomMateFinder.Features.Matching.LikeProfile;


using MediatR;
using Microsoft.AspNetCore.Mvc;


public static class LikeEndpoint
{
    public static void MapLikeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/like", async (
            LikeRequest request,
            IMediator mediator,
            IValidator<LikeRequest> validator) =>
        {
            await validator.ValidateAndThrowAsync(request);

            var result = await mediator.Send(new LikeCommand(request));
            return Results.Ok(new { success = result });
        });

    }
}
