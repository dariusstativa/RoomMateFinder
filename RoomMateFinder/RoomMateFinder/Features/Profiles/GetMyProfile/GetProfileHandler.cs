using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.GetMyProfile;

public class GetProfileHandler : IRequestHandler<GetProfileQuery, Profile?>
{
    private readonly AppDbContext _context;

    public GetProfileHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Profile?> Handle(GetProfileQuery query, CancellationToken ct)
    {
        return await _context.Profiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == query.UserId, ct);
    }
}