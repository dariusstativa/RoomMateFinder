namespace RoomMateFinder.Features.Conversations.Messaging.Message;
using MediatR;
    
public class SendMessageCommand : IRequest<MessageResponse>
{
    public Guid SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
}


