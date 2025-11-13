using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetProfileById;

public record GetProfileByIdQuery(Guid UserId) : IRequest<Profile?>;