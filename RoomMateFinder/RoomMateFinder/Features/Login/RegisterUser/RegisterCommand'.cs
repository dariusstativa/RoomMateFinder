using MediatR;
<<<<<<< HEAD
using RoomMateFinder.Features.Login.LoginUser;

namespace RoomMateFinder.Features.Login.RegisterUser;

public record RegisterCommand(RegisterRequest Request) : IRequest<LoginResponse>;
=======

namespace RoomMateFinder.Features.Login.RegisterUser;

public record RegisterCommand(RegisterRequest Request) : IRequest<Guid>;
>>>>>>> CleanFixBranch
