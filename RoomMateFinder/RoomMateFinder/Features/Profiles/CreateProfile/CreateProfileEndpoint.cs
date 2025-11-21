namespace RoomMateFinder.Features.Profiles.CreateProfile;

using MediatR;
using Microsoft.AspNetCore.Mvc;

public static class CreateProfileEndpoint
{
    public static void MapCreateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profiles/{userId:guid}", async (
            Guid userId,
            [FromBody] CreateProfileRequest request,
            IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateProfileCommand(userId, request));
            return Results.Created($"/profiles/{id}", id);
        });
    }
}
