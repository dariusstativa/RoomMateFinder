using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public static class LikeEndpoint
{
    public static void MapLikeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/like", async (
                HttpContext http,
                LikeRequest request,
                IMediator mediator,
                IValidator<LikeRequest> validator) =>
            {
                await validator.ValidateAndThrowAsync(request);

                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);

                var result = await mediator.Send(new LikeCommand(userId, request));

                return Results.Ok(new { success = result });
            })
            .RequireAuthorization();
    }
}