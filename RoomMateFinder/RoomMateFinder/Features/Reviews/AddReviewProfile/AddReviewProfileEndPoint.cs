using System.Security.Claims;
using MediatR;

namespace RoomMateFinder.Features.Reviews.AddReviewProfile;

public static class AddReviewForProfileEndpoint
{
    public static void MapAddReviewForProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profiles/{id:guid}/reviews",
                async (Guid id, AddReviewForProfileRequest body, HttpContext ctx, IMediator mediator) =>
                {
                    var reviewerId = Guid.Parse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                    var cmd = new AddReviewForProfileCommand(
                        ProfileId: id,
                        ReviewerId: reviewerId,
                        Rating: body.Rating,
                        Comment: body.Comment
                    );

                    var result = await mediator.Send(cmd);
                    return Results.Ok(result);
                })
            .RequireAuthorization();
    }
}