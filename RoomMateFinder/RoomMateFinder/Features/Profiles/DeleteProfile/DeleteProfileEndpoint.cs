using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoomMateFinder.Features.Profiles.DeleteProfile;

public static class DeleteProfileEndpoint
{
    public static void MapDeleteProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/profiles/{userId:guid}", async (
            Guid userId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteProfileCommand(userId));

            return result ? Results.NoContent() : Results.NotFound();
        });
    }
}