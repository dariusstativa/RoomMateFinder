using MediatR;
using RoomMateFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Features.Profiles.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, UpdateProfileRequest Request) : IRequest<bool>;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, bool>
{
    private readonly AppDbContext _context;
    public UpdateProfileHandler(AppDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateProfileCommand cmd, CancellationToken ct)
    {
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);
        if (profile == null) return false;

      
        profile.Bio = cmd.Request.Bio ?? profile.Bio;
        profile.SleepSchedule = cmd.Request.SleepSchedule ?? profile.SleepSchedule;
        profile.Cleanliness = cmd.Request.Cleanliness ?? profile.Cleanliness;
        profile.NoiseTolerance = cmd.Request.NoiseTolerance ?? profile.NoiseTolerance;
        profile.SmokingPreference = cmd.Request.SmokingPreference ?? profile.SmokingPreference;
        profile.PetPreference = cmd.Request.PetPreference ?? profile.PetPreference;
        profile.StudyHabits = cmd.Request.StudyHabits ?? profile.StudyHabits;

        await _context.SaveChangesAsync();
        return true;
    }
}