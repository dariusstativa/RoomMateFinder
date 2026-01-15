using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings.GetMyListings;

public static class GetMyListingsEndpoint
{
    public static void MapGetMyListingsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/room-listings/mine", async (
                HttpContext http,
                AppDbContext db,
                CancellationToken ct) =>
            {
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim.Value);

                var listings = await db.RoomListings
                    .Where(l => l.OwnerId == userId)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToListAsync(ct);

                return Results.Ok(listings);
            })
            .RequireAuthorization();
    }
}