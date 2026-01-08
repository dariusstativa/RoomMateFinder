using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;
using System.Security.Claims;


namespace RoomMateFinder.Features.Conversations.Messaging.Conversation;

    public static class GetOrCreateConversationEndpoint
    {
        public static void MapGetOrCreateConversation(this IEndpointRouteBuilder app)
        {
            app.MapPost("/conversations/with/{targetUserId:guid}", async (
                    Guid targetUserId,
                    HttpContext http,
                    AppDbContext db,
                    CancellationToken ct) =>
                {
                    var userIdClaim =
                        http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                        http.User.FindFirst("sub");

                    if (userIdClaim == null)
                        return Results.Unauthorized();

                    var currentUserId = Guid.Parse(userIdClaim.Value);

                    // Find existing conversation between the two users
                    var conversation = await db.Conversations.FirstOrDefaultAsync(c =>
                            (c.User1Id == currentUserId && c.User2Id == targetUserId) ||
                            (c.User1Id == targetUserId && c.User2Id == currentUserId),
                        ct);

                    if (conversation != null)
                        return Results.Ok(conversation.Id);

                    // Create if missing
                    conversation = new Domain.Entities.Conversation
                    {
                        Id = Guid.NewGuid(),
                        User1Id = currentUserId,
                        User2Id = targetUserId,
                        CreatedAt = DateTime.UtcNow
                    };

                    db.Conversations.Add(conversation);
                    await db.SaveChangesAsync(ct);

                    return Results.Ok(conversation.Id);
                })
                .RequireAuthorization();
        }
    }
