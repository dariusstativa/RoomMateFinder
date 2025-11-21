namespace RoomMateFinder.Domain.Entities;

public class Like
{
    public Guid Id { get; set; }
    public Guid LikerUserId { get; set; }
    public User LikerUser { get; set; }
    public Profile TargetProfile { get; set; }
    public Guid TargetProfileId { get; set; }
    public bool IsLike { get; set; }
    public DateTime CreatedAt { get; set; }
}
