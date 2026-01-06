namespace RoomMateFinder.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }

    // 🔑 LEGĂTURA CU CONVERSAȚIA
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = default!;

    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }

    public string Content { get; set; } = default!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public User Sender { get; set; } = default!;
    public User Receiver { get; set; } = default!;
}