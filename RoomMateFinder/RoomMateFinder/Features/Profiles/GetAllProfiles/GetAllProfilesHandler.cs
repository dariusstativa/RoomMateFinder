using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.GetAllProfiles;

public class GetAllProfilesHandler
    : IRequestHandler<GetAllProfilesQuery, List<Profile>>
{
    private readonly AppDbContext _db;

    public GetAllProfilesHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Profile>> Handle(
        GetAllProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        // Profiles already liked OR passed by this user
        var excludedProfileIds = _db.Likes
            .Where(l => l.LikerUserId == userId)
            .Select(l => l.TargetProfileId);

        return await _db.Profiles
            .AsNoTracking()
            .Where(p => p.UserId != userId)                 // not own profile
            .Where(p => !excludedProfileIds.Contains(p.Id)) // not liked/passed
            .OrderBy(p => p.FullName)
            .ToListAsync(cancellationToken);
    }
}