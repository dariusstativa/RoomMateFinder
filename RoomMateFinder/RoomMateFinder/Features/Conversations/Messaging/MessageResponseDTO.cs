namespace RoomMateFinder.Features.Conversations.Messaging;

public class MessageResponse
{
    public Guid Id { get; set; }

    
    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }

    public string Content { get; set; } = default!;
    public DateTime SentAt { get; set; }
}