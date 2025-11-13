using MediatR;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public record DislikeCommand(DislikeRequest Request) : IRequest<bool>;