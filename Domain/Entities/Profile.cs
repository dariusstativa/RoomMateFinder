using System.ComponentModel.DataAnnotations.Schema;

namespace RoomMateFinder.Domain.Entities;


public class Profile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }   // FK to User

    public string FullName { get; set; } = default!;
    public int Age { get; set; }
    public string Gender { get; set; } = default!;
    public string University { get; set; } = default!;
    public string Bio { get; set; } = default!;

    
    public string SleepSchedule { get; set; } = default!; 
    public string Cleanliness { get; set; } = default!;     
    public string NoiseTolerance { get; set; } = default!;  
    public string SmokingPreference { get; set; } = default!;
    public string PetPreference { get; set; } = default!;
    public string StudyHabits { get; set; } = default!;

    public bool IsOnboarded { get; set; }
    public DateTime? OnboardedAt { get; set; }
    public User User { get; set; } = default!;
    

}
