using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
public record GetMyProfileQuery(Guid UserId) : IRequest<Profile?>;
=======
public record GetProfileQuery(Guid UserId) : IRequest<Profile?>;
>>>>>>> Stashed changes
=======
public record GetProfileQuery(Guid UserId) : IRequest<Profile?>;
>>>>>>> Stashed changes
