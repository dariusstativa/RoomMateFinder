namespace RoomMateFinder.Features.Profiles.CreateProfile;

public record CreateProfileRequest(
    string FullName,
    int Age,
    string Gender,
    string University,
    string Bio,
    string SleepSchedule,
    string Cleanliness,
    string NoiseTolerance,
    string SmokingPreference,
    string PetPreference,
    string StudyHabits
);
