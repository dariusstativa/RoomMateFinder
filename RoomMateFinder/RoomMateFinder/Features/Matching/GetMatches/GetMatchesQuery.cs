using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Matching.GetMatches;

public record GetMatchesQuery(Guid UserId) : IRequest<List<Profile>>;