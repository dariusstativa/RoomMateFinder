using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, Profile?>
{
    private readonly AppDbContext _db;
    public GetMyProfileHandler(AppDbContext db) => _db = db;

    public async Task<Profile?> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        return await _db.Profiles
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
    }
}