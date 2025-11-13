using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.CompleteOnboarding;

public class CompleteOnboardingHandler : IRequestHandler<CompleteOnboardingCommand, bool>
{
    private readonly AppDbContext _db;
    public CompleteOnboardingHandler(AppDbContext db) => _db = db;

    public async Task<bool> Handle(CompleteOnboardingCommand request, CancellationToken ct)
    {
        var p = await _db.Profiles.FirstOrDefaultAsync(x => x.UserId == request.UserId, ct);
        if (p is null) return false;

        p.IsOnboarded = true;            
        p.OnboardedAt = DateTime.UtcNow;   


        await _db.SaveChangesAsync(ct);
        return true;
    }
}