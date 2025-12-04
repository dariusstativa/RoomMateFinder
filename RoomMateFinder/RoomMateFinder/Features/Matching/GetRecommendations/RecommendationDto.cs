namespace RoomMateFinder.Features.Matching.GetRecommendations;

public record RecommendationDto(
    Guid ProfileId,
    string FullName,
    int Age,
    string University,
    string Gender,
    int Rating,
    int ScoreDistance
);