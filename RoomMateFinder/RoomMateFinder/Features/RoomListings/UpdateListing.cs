using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings;

public static class UpdateListing
{
    public static void MapUpdateListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/listings/{id:guid}", async (AppDbContext db, Guid id, RoomListing updated) =>
        {
            var existing = await db.RoomListings.FindAsync(id);
            if (existing is null) return Results.NotFound();

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Address = updated.Address;
            existing.Price = updated.Price;
            existing.RoommatesCount = updated.RoommatesCount;
            existing.IsAvailable = updated.IsAvailable;
            existing.GenderPreference = updated.GenderPreference;

            await db.SaveChangesAsync();
            return Results.Ok(existing);
        });
    }
}