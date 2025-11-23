using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.GetAllProfiles;

public class GetAllProfilesHandler : IRequestHandler<GetAllProfilesQuery, List<Profile>>
{
    private readonly AppDbContext _db;

    public GetAllProfilesHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Profile>> Handle(GetAllProfilesQuery request, CancellationToken cancellationToken)
    {
        return await _db.Profiles
            .AsNoTracking()
            .OrderBy(p => p.FullName) 
            .ToListAsync(cancellationToken);
    }
}