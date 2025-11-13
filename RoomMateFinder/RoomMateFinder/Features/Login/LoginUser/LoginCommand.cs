using MediatR;

namespace RoomMateFinder.Features.Login.LoginUser;

public record LoginCommand(LoginRequest Request) : IRequest<Guid>;
