namespace RoomMateFinder.Client.Models;

public class CreateListingRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int RoommatesCount { get; set; }
    public string GenderPreference { get; set; } = "Any";
}

