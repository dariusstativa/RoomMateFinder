using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoomMateFinder.Features.LikeProfile.LikeRequest;
using System.Security.Claims;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public static class LikeEndpoint
{
    public static void MapLikeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/like", async (
                HttpContext http,
                LikeRequest request,
                IMediator mediator,
                IValidator<LikeRequest> validator) =>
            {
                await validator.ValidateAndThrowAsync(request);

                // Prioritate: NameIdentifier (frontend coleg)
                // Fallback: "sub" (standard JWT)
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                {
                    // Debugging (SUPER util dacă token-ul nu conține ce trebuie)
                    var claims = string.Join(", ", 
                        http.User.Claims.Select(c => $"{c.Type}={c.Value}"));

                    Console.WriteLine($"❌ User ID claim missing. Available claims: {claims}");

                    return Results.BadRequest(new
                    {
                        error = "User ID not found in token. Please log in again."
                    });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var result = await mediator.Send(new LikeCommand(userId, request));

                return Results.Ok(new { success = result });
            })
            .RequireAuthorization();
    }
}