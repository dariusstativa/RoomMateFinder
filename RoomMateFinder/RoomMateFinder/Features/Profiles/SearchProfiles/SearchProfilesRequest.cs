using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.Profiles.SearchProfiles;

public class SearchProfilesRequest : IRequest<List<Profile>>
{
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }

    public string? Gender { get; set; }
    public string? University { get; set; }

    public int? MinCleanliness { get; set; }
    public int? MaxCleanliness { get; set; }

    public int? MinNoiseTolerance { get; set; }
    public int? MaxNoiseTolerance { get; set; }

    public string? SmokingPreference { get; set; }
    public string? PetPreference { get; set; }
    public string? SleepSchedule { get; set; }
    public string? StudyHabits { get; set; }
}