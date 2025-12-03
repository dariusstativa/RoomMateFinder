using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.GetProfileById;

public class GetProfileByIdHandler : IRequestHandler<GetProfileByIdQuery, Profile?>
{
    private readonly AppDbContext _db;
<<<<<<< HEAD
    public GetProfileByIdHandler(AppDbContext db) => _db = db;
=======

    public GetProfileByIdHandler(AppDbContext db)
    {
        _db = db;
    }
>>>>>>> DariusBranch

    public async Task<Profile?> Handle(GetProfileByIdQuery request, CancellationToken ct)
    {
        return await _db.Profiles
<<<<<<< HEAD
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
=======
            .Include(p => p.User)  // poți păstra include, nu deranjează frontend-ul
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProfileId, ct);
>>>>>>> DariusBranch
    }
}