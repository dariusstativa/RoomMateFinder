using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public class GetProfileHandler : IRequestHandler<GetProfileQuery, Profile?>
{
    private readonly AppDbContext _db;

    public GetProfileHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Profile?> Handle(GetProfileQuery request, CancellationToken ct)
    {
        return await _db.Profiles
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
    }
}