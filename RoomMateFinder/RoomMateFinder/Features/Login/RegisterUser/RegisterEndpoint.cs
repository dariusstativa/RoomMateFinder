using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RoomMateFinder.Features.Login.RegisterUser;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (
            [FromBody] RegisterRequest request,
            IMediator mediator) =>
        {
            var response = await mediator.Send(new RegisterCommand(request));
            return Results.Ok(response);
        });
    }
}