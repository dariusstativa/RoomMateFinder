using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;
using System.Security.Claims;

namespace RoomMateFinder.Features.Messages;

public static class GetConversationMessagesEndpoint
{
    public static void MapGetConversationMessages(this IEndpointRouteBuilder app)
    {
        app.MapGet("/messages/conversation/{conversationId:guid}", async (
                Guid conversationId,
                HttpContext http,
                AppDbContext db,
                CancellationToken ct) =>
            {
                var userIdClaim =
                    http.User.FindFirst(ClaimTypes.NameIdentifier) ??
                    http.User.FindFirst("sub");

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var me = Guid.Parse(userIdClaim.Value);

                var conv = await db.Conversations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == conversationId, ct);

                if (conv is null)
                    return Results.NotFound();

                if (conv.User1Id != me && conv.User2Id != me)
                    return Results.Forbid();

                var msgs = await db.Messages
                    .AsNoTracking()
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync(ct);

                return Results.Ok(msgs);
            })
            .RequireAuthorization();
    }
}