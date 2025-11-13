namespace RoomMateFinder.Features.Profiles.UpdateProfile;

public record UpdateProfileRequest(
    string? Bio,
    string? SleepSchedule,
    string? Cleanliness,
    string? NoiseTolerance,
    string? SmokingPreference,
    string? PetPreference,
    string? StudyHabits
);
