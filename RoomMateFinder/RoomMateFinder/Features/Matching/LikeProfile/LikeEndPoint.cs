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

                var subClaim = http.User.FindFirst(ClaimTypes.NameIdentifier);
                if (subClaim == null)
                {
                    // Log all available claims for debugging
                    var claims = string.Join(", ", http.User.Claims.Select(c => $"{c.Type}={c.Value}"));
                    Console.WriteLine($"❌ 'sub' claim not found. Available claims: {claims}");
                    return Results.BadRequest(new { error = "User ID not found in token. Please log in again." });
                }

                var userId = Guid.Parse(subClaim.Value);

                var result = await mediator.Send(new LikeCommand(userId, request));

                return Results.Ok(new { success = result });
            })
            .RequireAuthorization();
    }
}