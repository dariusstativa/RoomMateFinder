using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

using MediatR;

public record LikeCommand(Guid UserId, LikeRequest Request) : IRequest<bool>;

