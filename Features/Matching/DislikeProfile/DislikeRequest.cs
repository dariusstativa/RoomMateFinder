namespace RoomMateFinder.Features.Matching.DislikeProfile;

public class DislikeRequest
{
    public Guid LikerUserId { get; set; }
    public Guid TargetProfileId { get; set; }
}