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

    // 🔐 Extrage userId EXACT ca în controller
    private Guid GetUserId()
    {
        var id = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(id))
            throw new HubException("Invalid JWT: missing NameIdentifier claim");

        return Guid.Parse(id);
    }

    // format conversationId: "{guid1}|{guid2}"
    public async Task JoinConversation(string conversationId)
    {
        var me = GetUserId();

        var parts = conversationId.Split('|', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new HubException("Invalid conversationId format. Use '{guid1}|{guid2}'");

        if (!Guid.TryParse(parts[0], out var u1) ||
            !Guid.TryParse(parts[1], out var u2))
            throw new HubException("Invalid conversationId GUIDs");

        if (me != u1 && me != u2)
            throw new HubException("Not allowed to join this conversation");

        var other = me == u1 ? u2 : u1;

        // verificăm că există mesaje între ei
        var allowed = await _db.Messages.AnyAsync(m =>
            (m.SenderId == me && m.ReceiverId == other) ||
            (m.SenderId == other && m.ReceiverId == me));

        if (!allowed)
            throw new HubException("Conversation does not exist");

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public Task LeaveConversation(string conversationId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);

    // 🔔 grup personal (pentru notificări viitoare)
    public override async Task OnConnectedAsync()
    {
        var me = GetUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, me.ToString());
        await base.OnConnectedAsync();
    }
}
