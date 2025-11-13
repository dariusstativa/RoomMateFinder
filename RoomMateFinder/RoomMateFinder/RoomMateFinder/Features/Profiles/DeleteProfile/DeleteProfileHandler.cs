using MediatR;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Profiles.DeleteProfile;

public class DeleteProfileHandler : IRequestHandler<DeleteProfileCommand, bool>
{
    private readonly AppDbContext _db;

    public DeleteProfileHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _db.Profiles.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (profile is null)
            return false; 

        _db.Profiles.Remove(profile);

        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}