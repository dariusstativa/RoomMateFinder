using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings;

public static class GetListingById
{
    public static void MapGetListingByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/listings/{id:guid}", async (AppDbContext db, Guid id) =>
        {
            var listing = await db.RoomListings
                .Include(l => l.Owner)
                .FirstOrDefaultAsync(l => l.Id == id);

            return listing is not null ? Results.Ok(listing) : Results.NotFound();
        });
    }
}