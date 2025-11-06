using MediatR;

namespace RoomMateFinder.Features.Profiles.UpdateProfile;

public static class UpdateProfileEndpoint
{
    public static void MapUpdateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/profiles/{userId:guid}", async (Guid userId, UpdateProfileRequest req, IMediator mediator) =>
        {
            var success = await mediator.Send(new UpdateProfileCommand(userId, req));
            return success ? Results.NoContent() : Results.NotFound();
        });
    }
}
