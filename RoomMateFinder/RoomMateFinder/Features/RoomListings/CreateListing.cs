using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings;

public static class CreateListing
{
    public static void MapCreateListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/listings", async (AppDbContext db, RoomListing listing) =>
        {
            listing.Id = Guid.NewGuid();
            listing.CreatedAt = DateTime.UtcNow;
            db.RoomListings.Add(listing);
            await db.SaveChangesAsync();

            return Results.Created($"/listings/{listing.Id}", listing);
        });
    }
}