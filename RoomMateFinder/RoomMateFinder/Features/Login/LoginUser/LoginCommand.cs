using MediatR;

namespace RoomMateFinder.Features.Login.LoginUser;

<<<<<<< HEAD
public record LoginCommand(LoginRequest Request) : IRequest<Guid>;
=======
public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;

>>>>>>> DariusBranch
