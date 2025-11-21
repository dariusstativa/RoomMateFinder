using MediatR;

namespace RoomMateFinder.Features.Matching.GetRecommendations;

public record GetRecommendationsQuery(Guid UserId) 
    : IRequest<List<RecommendationDto>>;