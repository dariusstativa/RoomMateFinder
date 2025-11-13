namespace RoomMateFinder.Features.RoomListings.CreateListing;

public class CreateListingRequest
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Address { get; set; } = default!;
    public decimal Price { get; set; }
    public int RoommatesCount { get; set; }
    public string GenderPreference { get; set; } = "Any";
}