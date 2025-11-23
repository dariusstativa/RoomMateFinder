using MediatR;
using FluentValidation;
using System.Security.Claims;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public static class DislikeEndpoint
{
    public static void MapDislikeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/dislike", async (
                HttpContext http,
                DislikeRequest request,
                IMediator mediator,
                IValidator<DislikeRequest> validator) =>
            {
                await validator.ValidateAndThrowAsync(request);

                // încercăm mai întâi NameIdentifier, apoi fallback pe "sub"
                var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier) 
                                  ?? http.User.FindFirst("sub");

                if (userIdClaim is null)
                {
                    return Results.Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var result = await mediator.Send(new DislikeCommand(userId, request));

                return Results.Ok(new { success = result });
            })
            .RequireAuthorization();
    }
}