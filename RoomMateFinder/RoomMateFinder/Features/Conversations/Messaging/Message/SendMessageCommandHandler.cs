using MediatR;
using Microsoft.AspNetCore.SignalR;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Conversations;
using RoomMateFinder.Features.Conversations.Messaging;
using RoomMateFinder.Features.Conversations.Messaging.Message;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Application.Messages.Handlers
{
    public class SendMessageCommandHandler
        : IRequestHandler<SendMessageCommand, MessageResponse>
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hub;

        public SendMessageCommandHandler(
            AppDbContext context,
            IHubContext<ChatHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<MessageResponse> Handle(
            SendMessageCommand request,
            CancellationToken cancellationToken)
        {
            var message = new Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new MessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                SentAt = message.SentAt
            };

            // 🔥 IDENTIC cu frontend + ChatHub
            var conversationId = BuildConversationId(
                request.SenderId,
                request.ReceiverId
            );

            await _hub
                .Clients
                .Group(conversationId)
                .SendAsync("ReceiveMessage", response, cancellationToken);

            return response;
        }

        private static string BuildConversationId(Guid u1, Guid u2)
        {
            return u1.CompareTo(u2) < 0
                ? $"{u1}|{u2}"
                : $"{u2}|{u1}";
        }
    }
}
