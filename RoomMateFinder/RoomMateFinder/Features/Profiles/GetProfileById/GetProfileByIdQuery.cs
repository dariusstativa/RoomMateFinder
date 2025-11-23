using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetProfileById;

<<<<<<< HEAD
public record GetProfileByIdQuery(Guid UserId) : IRequest<Profile?>;
=======
public record GetProfileByIdQuery(Guid ProfileId) : IRequest<ProfileDto?>;
>>>>>>> CleanFixBranch
