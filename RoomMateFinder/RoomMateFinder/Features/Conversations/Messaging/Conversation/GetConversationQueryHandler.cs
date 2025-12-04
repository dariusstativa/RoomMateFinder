using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Features.Conversations.Messaging;
using RoomMateFinder.Features.Conversations.Messaging.Conversation;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Application.Messages.Handlers
{
    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, List<MessageResponse>>
    {
        private readonly AppDbContext _context;

        public GetConversationQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MessageResponse>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == request.User1 && m.ReceiverId == request.User2) ||
                    (m.SenderId == request.User2 && m.ReceiverId == request.User1))
                .OrderBy(m => m.SentAt)
                .ToListAsync(cancellationToken);

            return messages.Select(m => new MessageResponse
            {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                SentAt = m.SentAt
            }).ToList();
        }
    }
}