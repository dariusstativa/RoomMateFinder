using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Matching.GetRecommendations;

public class GetRecommendationsHandler 
    : IRequestHandler<GetRecommendationsQuery, List<RecommendationDto>>
{
    private readonly AppDbContext _db;

    public GetRecommendationsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<RecommendationDto>> Handle(
        GetRecommendationsQuery request,
        CancellationToken ct)
    {
        var me = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (me?.Profile == null)
            return [];

        var myProfile = me.Profile;

        var excluded = await _db.Likes
            .Where(l => l.LikerUserId == request.UserId)
            .Select(l => l.TargetProfileId)
            .ToListAsync(ct);

        var candidates = await _db.Profiles
            .Include(p => p.User)
            .Where(p => p.Id != myProfile.Id)
            .Where(p => !excluded.Contains(p.Id))
            .Where(p => p.University == myProfile.University)
            .Where(p => p.Gender == myProfile.Gender || myProfile.Gender == "Any")
            .ToListAsync(ct);

        var list = candidates
            .Select(c => new RecommendationDto(
                ProfileId: c.Id,
                FullName: c.FullName,
                Age: c.Age,
                University: c.University,
                Gender: c.Gender,
                Rating: c.User.Rating,
                ScoreDistance: Math.Abs(c.User.Rating - me.Rating)
            ))
            .OrderBy(r => r.ScoreDistance)
            .ToList();

        return list;
    }
}