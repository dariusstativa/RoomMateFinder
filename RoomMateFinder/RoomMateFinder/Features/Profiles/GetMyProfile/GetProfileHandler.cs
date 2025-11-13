using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======

namespace RoomMateFinder.Features.Profiles.GetMyProfile;
>>>>>>> Stashed changes

namespace RoomMateFinder.Features.Profiles.GetMyProfile;
>>>>>>> Stashed changes

namespace RoomMateFinder.Features.Profiles.GetMyProfile;

public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, Profile?>
{
    private readonly AppDbContext _db;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public GetMyProfileHandler(AppDbContext db) => _db = db;

    public async Task<Profile?> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
=======

    public GetProfileHandler(AppDbContext db)
    {
=======

    public GetProfileHandler(AppDbContext db)
    {
>>>>>>> Stashed changes
        _db = db;
    }

    public async Task<Profile?> Handle(GetProfileQuery request, CancellationToken ct)
    {
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        return await _db.Profiles
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
    }
}