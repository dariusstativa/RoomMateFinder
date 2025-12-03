using MediatR;
<<<<<<< HEAD
=======
using FluentValidation;
using System.Security.Claims;
>>>>>>> DariusBranch

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public static class DislikeEndpoint
{
    public static void MapDislikeEndpoint(this IEndpointRouteBuilder app)
    {
<<<<<<< HEAD
        app.MapPost("/dislike", async (Guid likerId, Guid targetId, IMediator mediator) =>
        {
            bool ok = await mediator.Send(new DislikeCommand(likerId, targetId));
            return ok ? Results.Ok() : Results.BadRequest();
        });
=======
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
>>>>>>> DariusBranch
    }
}