using MediatR;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

<<<<<<< HEAD
public record DislikeCommand(Guid LikerUserId, Guid TargetProfileId) : IRequest<bool>;
=======
public record DislikeCommand(Guid UserId, DislikeRequest Request) : IRequest<bool>;
>>>>>>> DariusBranch
