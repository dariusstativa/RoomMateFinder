namespace RoomMateFinder.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public Guid ReviewerId { get; set; }
    public User Reviewer { get; set; } = default!;

   
    public Guid? TargetUserId { get; set; }
    public User? TargetUser { get; set; }

    public Guid? RoomListingId { get; set; }
    public RoomListing? RoomListing { get; set; }

    public Guid? ProfileId { get; set; }
    public Profile? Profile { get; set; }

    
    public int Rating { get; set; }
    public string Comment { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}