using MediatR;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.GetProfileById;

public class GetProfileByIdHandler : IRequestHandler<GetProfileByIdQuery, Profile?>
{
    private readonly AppDbContext _db;
    public GetProfileByIdHandler(AppDbContext db) => _db = db;

    public async Task<Profile?> Handle(GetProfileByIdQuery request, CancellationToken ct)
    {
        return await _db.Profiles
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
=======
using RoomMateFinder.Infrastructure.Persistence;
using RoomMateFinder.Features.Profiles;

namespace RoomMateFinder.Features.Profiles.GetProfileById;

public class GetProfileByIdHandler : IRequestHandler<GetProfileByIdQuery, ProfileDto?>
{
    private readonly AppDbContext _db;

    public GetProfileByIdHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProfileDto?> Handle(GetProfileByIdQuery request, CancellationToken ct)
    {
        var profile = await _db.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProfileId, ct);

        if (profile is null)
            return null;

        return new ProfileDto(
            profile.Id,
            profile.UserId,
            profile.FullName,
            profile.Age,
            profile.Gender,
            profile.University,
            profile.Bio,
            profile.SleepSchedule,
            profile.Cleanliness,
            profile.NoiseTolerance,
            profile.SmokingPreference,
            profile.PetPreference,
            profile.StudyHabits
        );
>>>>>>> CleanFixBranch
    }
}