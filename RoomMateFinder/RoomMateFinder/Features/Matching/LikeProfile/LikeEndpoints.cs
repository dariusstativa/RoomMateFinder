using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public static class LikeEndpoints
{
    public static void MapLikeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/like", async (
                HttpContext http,
                LikeRequest request,
                IMediator mediator,
                IValidator<LikeRequest> validator,
                CancellationToken ct) =>
            {
                await validator.ValidateAndThrowAsync(request);

                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim.Value);

                var ok = await mediator.Send(new LikeCommand(userId, request), ct);

                return ok ? Results.Ok() : Results.BadRequest();
            })
            .RequireAuthorization();
    }
}