namespace RoomMateFinder.Features.Profiles.CompleteOnboarding;

public record CompleteOnboardingRequest(
    string? Personality = null,
    string? LifestyleNotes = null
);
