namespace RoomMateFinder.Features.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = default!;
    public int Rating { get; set; }
    public string Comment { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}