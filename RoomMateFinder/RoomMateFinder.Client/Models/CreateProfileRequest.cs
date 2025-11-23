namespace RoomMateFinder.Client.Models;

public class CreateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string SleepSchedule { get; set; } = string.Empty;
    public string Cleanliness { get; set; } = string.Empty;
    public string NoiseTolerance { get; set; } = string.Empty;
    public string SmokingPreference { get; set; } = string.Empty;
    public string PetPreference { get; set; } = string.Empty;
    public string StudyHabits { get; set; } = string.Empty;
}

