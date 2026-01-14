using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Messaging.Conversation;

public static class GetOrCreateListingConversationEndpoint
{
    public static void MapGetOrCreateListingConversation(this IEndpointRouteBuilder app)
    {
        app.MapPost("/conversations/listing/{listingId:guid}", async (
            Guid listingId,
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

            // 1️⃣ Luăm listing-ul
            var listing = await db.RoomListings
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == listingId, ct);

            if (listing is null)
                return Results.NotFound("Listing not found.");

            if (listing.OwnerId == userId)
                return Results.BadRequest("You cannot message yourself.");

            // 2️⃣ Căutăm conversația existentă (ORICARE între cei 2 useri)
            // ✅ FIX: Nu mai verificăm ListingId, căutăm ORICE conversație între cei 2
            var conversation = await db.Conversations.FirstOrDefaultAsync(c =>
                (c.User1Id == userId && c.User2Id == listing.OwnerId) ||
                (c.User1Id == listing.OwnerId && c.User2Id == userId),
                ct);

            // 3️⃣ Dacă există deja → returnăm ID-ul ei
            if (conversation is not null)
            {
                // ✅ Opțional: actualizăm ListingId dacă e null
                if (conversation.ListingId is null)
                {
                    conversation.ListingId = listingId;
                    await db.SaveChangesAsync(ct);
                }
                
                return Results.Ok(conversation.Id);
            }

            // 4️⃣ Dacă nu există → o creăm
            conversation = new Domain.Entities.Conversation
            {
                Id = Guid.NewGuid(),
                User1Id = userId,
                User2Id = listing.OwnerId,
                ListingId = listingId,
                CreatedAt = DateTime.UtcNow
            };

            db.Conversations.Add(conversation);
            await db.SaveChangesAsync(ct);

            // 5️⃣ Returnăm conversationId
            return Results.Ok(conversation.Id);
        })
        .RequireAuthorization();
    }
}