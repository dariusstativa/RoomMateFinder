namespace RoomMateFinder.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }

    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }

    // ✅ NOU: conversație legată de un listing (null = chat clasic)
    public Guid? ListingId { get; set; }

    public DateTime CreatedAt { get; set; }
}