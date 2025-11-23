using MediatR;
using FluentValidation;
<<<<<<< HEAD
using System.Security.Claims;
=======
>>>>>>> CleanFixBranch

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

<<<<<<< HEAD
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
=======
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);
>>>>>>> CleanFixBranch

                var result = await mediator.Send(new DislikeCommand(userId, request));

                return Results.Ok(new { success = result });
            })
            .RequireAuthorization();
    }
}