using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings;

public static class GetAllListings
{
    public static void MapGetAllListingsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/listings", async (AppDbContext db) =>
        {
            var listings = await db.RoomListings
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return Results.Ok(listings);
        });
    }
}