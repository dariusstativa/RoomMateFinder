namespace RoomMateFinder.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid TargetUserId { get; set; }
    public int Rating { get; set; }     
    public string Comment { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    public User Reviewer { get; set; } = default!;
    public User TargetUser { get; set; } = default!;
}