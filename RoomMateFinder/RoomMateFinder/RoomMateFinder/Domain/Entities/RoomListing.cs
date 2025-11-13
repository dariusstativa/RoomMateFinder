namespace RoomMateFinder.Domain.Entities;


public class RoomListing
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }       
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Address { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

   
    public int RoommatesCount { get; set; }
    public string GenderPreference { get; set; } = "Any";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

   
    public User Owner { get; set; } = default!;
}
