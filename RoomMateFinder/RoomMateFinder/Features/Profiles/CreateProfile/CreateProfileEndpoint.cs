namespace RoomMateFinder.Features.Profiles.CreateProfile;

using MediatR;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
=======
using System.Security.Claims;
>>>>>>> DariusBranch

public static class CreateProfileEndpoint
{
    public static void MapCreateProfileEndpoint(this IEndpointRouteBuilder app)
    {
<<<<<<< HEAD
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
=======
        app.MapPost("/profiles", async (
                HttpContext http,
                [FromBody] CreateProfileRequest request,
                IMediator mediator) =>
            {
                
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                {
                    var claims = string.Join(", ",
                        http.User.Claims.Select(c => $"{c.Type}={c.Value}"));
                    Console.WriteLine($"❌ User ID claim missing. Available claims: {claims}");

                    return Results.BadRequest(new
                    {
                        error = "User ID not found in token. Please log in again."
                    });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var id = await mediator.Send(new CreateProfileCommand(userId, request));

                return Results.Created($"/profiles/{id}", id);
            })
            .RequireAuthorization();
    }
}
>>>>>>> DariusBranch
