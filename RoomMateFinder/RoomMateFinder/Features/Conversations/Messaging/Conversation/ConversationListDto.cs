namespace RoomMateFinder.Features.Messaging.Conversation;

public class ConversationListDto
{
    public Guid Id { get; set; }
    public Guid OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;
    public string? LastMessage { get; set; }
    public DateTime LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public Guid? ListingId { get; set; }
    public string? ListingTitle { get; set; }
}