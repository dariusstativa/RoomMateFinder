

using MediatR;

namespace RoomMateFinder.Features.Conversations.Messaging.Conversation
{
    public class GetConversationQuery : IRequest<List<MessageResponse>>
    {
        public Guid User1 { get; set; }
        public Guid User2 { get; set; }
    }
}
