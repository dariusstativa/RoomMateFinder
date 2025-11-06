using Microsoft.AspNetCore.Builder;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings;

public static class DeleteListing
{
    public static void MapDeleteListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/listings/{id:guid}", async (AppDbContext db, Guid id) =>
        {
            var listing = await db.RoomListings.FindAsync(id);
            if (listing is null) return Results.NotFound();

            db.RoomListings.Remove(listing);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}