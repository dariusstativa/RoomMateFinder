namespace RoomMateFinder.Features.Conversations.Messaging;

public class SendMessageDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
}
