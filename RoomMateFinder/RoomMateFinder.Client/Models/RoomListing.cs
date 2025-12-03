namespace RoomMateFinder.Client.Models;

public class RoomListing
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int RoommatesCount { get; set; }
    public string GenderPreference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

