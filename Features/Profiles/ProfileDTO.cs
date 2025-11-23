namespace RoomMateFinder.Features.Profiles;

public record ProfileDto(
    Guid Id,
    Guid UserId,
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