namespace RoomMateFinder.Features.LikeProfile.LikeRequest;

public record LikeRequest(Guid LikerUserId, Guid TargetProfileId);