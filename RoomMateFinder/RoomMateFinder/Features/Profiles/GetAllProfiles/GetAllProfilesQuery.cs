using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetAllProfiles;

public record GetAllProfilesQuery(Guid UserId) 
    : IRequest<List<Profile>>;