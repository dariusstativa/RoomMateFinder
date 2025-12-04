using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Conversations.Messaging;
using RoomMateFinder.Features.Conversations.Messaging.Message;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Application.Messages.Handlers
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageResponse>
    {
        private readonly AppDbContext _context;

        public SendMessageCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
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

            return new MessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                SentAt = message.SentAt
            };
        }
    }
}