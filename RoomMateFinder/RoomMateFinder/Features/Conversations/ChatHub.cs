using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;
using System.Security.Claims;

namespace RoomMateFinder.Features.Conversations;

[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _db;

    public ChatHub(AppDbContext db)
    {
        _db = db;
    }

    private Guid GetUserId()
    {
        var id =
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            Context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(id))
            throw new HubException("Invalid JWT: missing user id claim");

        return Guid.Parse(id);
    }


    public async Task JoinConversation(Guid conversationId)
    {
        var me = GetUserId();

        var conversation = await _db.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            throw new HubException("Conversation does not exist");

        
        if (conversation.User1Id != me && conversation.User2Id != me)
            throw new HubException("Not allowed to join this conversation");

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            conversationId.ToString()
        );
    }

    public Task LeaveConversation(Guid conversationId) =>
        Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            conversationId.ToString()
        );

    public override async Task OnConnectedAsync()
    {
        var me = GetUserId();

        // grup personal (pentru notificări, opțional)
        await Groups.AddToGroupAsync(Context.ConnectionId, me.ToString());

        await base.OnConnectedAsync();
    }
}