using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.SearchProfiles;

public class SearchProfilesHandler : IRequestHandler<SearchProfilesRequest, List<Profile>>
{
    private readonly AppDbContext _db;

    public SearchProfilesHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Profile>> Handle(SearchProfilesRequest req, CancellationToken ct)
    {
        var query = _db.Profiles.AsQueryable();

        // AGE
        if (req.MinAge.HasValue)
            query = query.Where(x => x.Age >= req.MinAge.Value);

        if (req.MaxAge.HasValue)
            query = query.Where(x => x.Age <= req.MaxAge.Value);

        // GENDER
        if (!string.IsNullOrWhiteSpace(req.Gender))
            query = query.Where(x => x.Gender == req.Gender);

        // UNIVERSITY
        if (!string.IsNullOrWhiteSpace(req.University))
            query = query.Where(x => x.University == req.University);

        // CLEANLINESS (string → numeric)
        if (req.MinCleanliness.HasValue)
            query = query.Where(x => int.Parse(x.Cleanliness) >= req.MinCleanliness.Value);

        if (req.MaxCleanliness.HasValue)
            query = query.Where(x => int.Parse(x.Cleanliness) <= req.MaxCleanliness.Value);

        // NOISE
        if (req.MinNoiseTolerance.HasValue)
            query = query.Where(x => int.Parse(x.NoiseTolerance) >= req.MinNoiseTolerance.Value);

        if (req.MaxNoiseTolerance.HasValue)
            query = query.Where(x => int.Parse(x.NoiseTolerance) <= req.MaxNoiseTolerance.Value);

        // SMOKING
        if (!string.IsNullOrWhiteSpace(req.SmokingPreference))
            query = query.Where(x => x.SmokingPreference == req.SmokingPreference);

        // PET
        if (!string.IsNullOrWhiteSpace(req.PetPreference))
            query = query.Where(x => x.PetPreference == req.PetPreference);

        // SLEEP SCHEDULE
        if (!string.IsNullOrWhiteSpace(req.SleepSchedule))
            query = query.Where(x => x.SleepSchedule == req.SleepSchedule);

        // STUDY HABITS
        if (!string.IsNullOrWhiteSpace(req.StudyHabits))
            query = query.Where(x => x.StudyHabits == req.StudyHabits);

        return await query.ToListAsync(ct);
    }
}
