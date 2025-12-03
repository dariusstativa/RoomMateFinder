using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RoomMateFinder.Features.Profiles.DeleteProfile;

public static class DeleteProfileEndpoint
{
    public static void MapDeleteProfileEndpoint(this IEndpointRouteBuilder app)
    {
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
    }
}