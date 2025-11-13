using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Matching.GetMatches;

public class GetMatchesHandler : IRequestHandler<GetMatchesQuery, List<Profile>>
{
    private readonly AppDbContext _db;

    public GetMatchesHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Profile>> Handle(GetMatchesQuery request, CancellationToken ct)
    {
        
        var myProfileId = await _db.Profiles
            .Where(p => p.UserId == request.UserId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (myProfileId == Guid.Empty)
            return [];

        
        var givenLikesQuery = _db.Likes
            .Include(l => l.TargetProfile)
            .Where(l => l.LikerUserId == request.UserId && l.IsLike);

       
        var matches = await givenLikesQuery
            .Where(myLike => _db.Likes.Any(other =>
                    other.IsLike &&
                    other.TargetProfile.UserId == request.UserId &&   
                    other.LikerUserId == myLike.TargetProfile.UserId  
            ))
            .Select(l => l.TargetProfile)
            .Distinct()
            .ToListAsync(ct);

        return matches;
    }
}