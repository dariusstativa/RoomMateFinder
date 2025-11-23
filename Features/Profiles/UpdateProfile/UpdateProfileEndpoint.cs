using MediatR;

namespace RoomMateFinder.Features.Profiles.UpdateProfile;

public static class UpdateProfileEndpoint
{
    public static void MapUpdateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/profiles", async (HttpContext http, UpdateProfileRequest req, IMediator mediator) =>
            {
                var userId = Guid.Parse(http.User.FindFirst("sub")!.Value);

                var success = await mediator.Send(new UpdateProfileCommand(userId, req));

                return success ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization();
    }
}