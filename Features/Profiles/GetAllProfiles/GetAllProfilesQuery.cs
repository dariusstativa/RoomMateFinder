using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.GetAllProfiles;

public record GetAllProfilesQuery() : IRequest<List<Profile>>;