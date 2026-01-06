namespace RoomMateFinder.Features.Conversations.Messaging.Message;

using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Conversations;
using RoomMateFinder.Infrastructure.Persistence;

public class SendMessageCommandHandler
    : IRequestHandler<SendMessageCommand, MessageResponse>
{
    private readonly AppDbContext _db;
    private readonly IHubContext<ChatHub> _hub;

    public SendMessageCommandHandler(AppDbContext db, IHubContext<ChatHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    public async Task<MessageResponse> Handle(
        SendMessageCommand request,
        CancellationToken ct)
    {
        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId, ct);

        if (conversation == null)
            throw new InvalidOperationException("Conversation does not exist");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = request.SenderId,
            ReceiverId = request.SenderId == conversation.User1Id
                ? conversation.User2Id
                : conversation.User1Id,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync(ct);

        var response = new MessageResponse
        {
            Id = message.Id,
            ConversationId = conversation.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Content = message.Content,
            SentAt = message.SentAt
        };

        await _hub.Clients
            .Group(conversation.Id.ToString())
            .SendAsync("ReceiveMessage", response, ct);

        return response;
    }

}
