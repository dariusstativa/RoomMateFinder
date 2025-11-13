namespace RoomMateFinder.Features.LikeProfile.LikeRequest;
public class LikeRequest
{
    public Guid LikerUserId { get; set; }
    public Guid TargetProfileId { get; set; }
}