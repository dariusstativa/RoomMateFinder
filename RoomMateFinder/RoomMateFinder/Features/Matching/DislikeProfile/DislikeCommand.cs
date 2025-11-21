using MediatR;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public record DislikeCommand(Guid LikerUserId, Guid TargetProfileId) : IRequest<bool>;