using MediatR;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
=======
using System.Security.Claims;
>>>>>>> DariusBranch

namespace RoomMateFinder.Features.Profiles.DeleteProfile;

public static class DeleteProfileEndpoint
{
    public static void MapDeleteProfileEndpoint(this IEndpointRouteBuilder app)
    {
<<<<<<< HEAD
        app.MapDelete("/profiles/{userId:guid}", async (
            Guid userId,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteProfileCommand(userId));

            return result ? Results.NoContent() : Results.NotFound();
        });
=======
        app.MapDelete("/profiles", async (
                HttpContext http,
                IMediator mediator) =>
            {
                // Căutăm întâi ClaimTypes.NameIdentifier (frontend coleg),
                // apoi "sub" (standard JWT).
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                {
                    // Debug info dacă ceva e broken în token
                    var claims = string.Join(", ",
                        http.User.Claims.Select(c => $"{c.Type}={c.Value}"));
                    Console.WriteLine($"❌ No userId in token. Claims: {claims}");

                    return Results.BadRequest(new
                    {
                        error = "User ID not found in token. Please login again."
                    });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var result = await mediator.Send(new DeleteProfileCommand(userId));

                return result ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization();
>>>>>>> DariusBranch
    }
}