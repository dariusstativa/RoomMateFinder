using MediatR;

namespace RoomMateFinder.Features.Matching.DislikeProfile;

public record DislikeCommand(Guid UserId, DislikeRequest Request) : IRequest<bool>;
