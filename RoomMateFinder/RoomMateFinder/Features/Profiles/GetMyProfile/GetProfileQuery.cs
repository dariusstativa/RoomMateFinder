using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public record GetMyProfileQuery(Guid UserId) : IRequest<Profile?>;