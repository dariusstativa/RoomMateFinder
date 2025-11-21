using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public record GetProfileQuery(Guid UserId) : IRequest<Profile?>;