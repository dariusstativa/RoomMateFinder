using MediatR;

namespace RoomMateFinder.Features.Login.RegisterUser;

public record RegisterCommand(RegisterRequest Request) : IRequest<Guid>;