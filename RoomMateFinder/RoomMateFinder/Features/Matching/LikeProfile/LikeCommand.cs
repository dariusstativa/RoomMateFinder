using MediatR;
using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public record LikeCommand(LikeRequest Request) : IRequest<bool>;