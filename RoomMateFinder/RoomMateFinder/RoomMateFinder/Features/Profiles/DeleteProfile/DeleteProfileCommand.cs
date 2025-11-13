using MediatR;

namespace RoomMateFinder.Features.Profiles.DeleteProfile;

public record DeleteProfileCommand(Guid UserId) : IRequest<bool>;