namespace RoomMateFinder.Features.Profiles.CreateProfile;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public static class CreateProfileEndpoint
{
    public static void MapCreateProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profiles", async (
                HttpContext http,
                [FromBody] CreateProfileRequest request,
                IMediator mediator) =>
            {
               
                var userId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                
                var id = await mediator.Send(new CreateProfileCommand(userId, request));

                return Results.Created($"/profiles/{id}", id);
            })
            .RequireAuthorization();
    }
}
