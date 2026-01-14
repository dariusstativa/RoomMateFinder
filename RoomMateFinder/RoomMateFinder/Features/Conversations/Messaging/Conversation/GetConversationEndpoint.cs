using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Messaging.Conversation;

public static class GetConversationsEndpoint
{
    public static void MapGetConversations(this IEndpointRouteBuilder app)
    {
        app.MapGet("/conversations", async (
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

            var conversations = await db.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Select(c => new
                {
                    c.Id,
                    OtherUserId = c.User1Id == userId ? c.User2Id : c.User1Id,
                    c.ListingId,
                    c.CreatedAt
                })
                .ToListAsync(ct);

            var result = new List<ConversationListDto>();

            foreach (var conv in conversations)
            {
                // Găsim numele din Profile
                var otherUserProfile = await db.Profiles
                    .Where(p => p.UserId == conv.OtherUserId)
                    .Select(p => p.FullName)
                    .FirstOrDefaultAsync(ct);

                // Găsim ultimul mesaj
                var lastMessage = await db.Messages
                    .Where(m => m.ConversationId == conv.Id)
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new { m.Content, m.SentAt })
                    .FirstOrDefaultAsync(ct);

                // Găsim titlul listing-ului dacă există
                string? listingTitle = null;
                if (conv.ListingId.HasValue)
                {
                    listingTitle = await db.RoomListings
                        .Where(l => l.Id == conv.ListingId.Value)
                        .Select(l => l.Title)
                        .FirstOrDefaultAsync(ct);
                }

                result.Add(new ConversationListDto
                {
                    Id = conv.Id,
                    OtherUserId = conv.OtherUserId,
                    OtherUserName = otherUserProfile ?? "Unknown User",
                    LastMessage = lastMessage?.Content,
                    LastMessageAt = lastMessage?.SentAt ?? conv.CreatedAt,
                    UnreadCount = 0, 
                    ListingId = conv.ListingId,
                    ListingTitle = listingTitle
                });
            }

            return Results.Ok(result.OrderByDescending(r => r.LastMessageAt));
        })
        .RequireAuthorization();
    }
}