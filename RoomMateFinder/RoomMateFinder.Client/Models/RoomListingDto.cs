namespace RoomMateFinder.Client.Models;

public class RoomListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal Price { get; set; }
}