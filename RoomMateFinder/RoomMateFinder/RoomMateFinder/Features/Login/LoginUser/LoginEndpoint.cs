using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoomMateFinder.Features.Login.LoginUser;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            [FromBody] LoginRequest request,
            IMediator mediator) =>
        {
            var userId = await mediator.Send(new LoginCommand(request));
            return Results.Ok(new { UserId = userId });
        });
    }
}