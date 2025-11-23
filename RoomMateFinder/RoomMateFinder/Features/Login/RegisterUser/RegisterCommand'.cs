using MediatR;
using RoomMateFinder.Features.Login.LoginUser;

namespace RoomMateFinder.Features.Login.RegisterUser;

public record RegisterCommand(RegisterRequest Request) : IRequest<LoginResponse>;
