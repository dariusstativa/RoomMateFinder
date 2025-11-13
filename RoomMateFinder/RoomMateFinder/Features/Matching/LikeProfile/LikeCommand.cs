using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

using MediatR;

public record LikeCommand(LikeRequest Request) : IRequest<bool>;
