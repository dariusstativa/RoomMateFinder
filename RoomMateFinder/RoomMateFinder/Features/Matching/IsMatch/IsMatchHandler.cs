using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

public class IsMatchHandler : IRequestHandler<IsMatchQuery, bool>
{
    private readonly AppDbContext _db;

    public IsMatchHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(IsMatchQuery request, CancellationToken ct)
    {
        var userId = request.OtherUserId;

        return await _db.Likes.AnyAsync(l1 =>
                l1.LikerUserId == request.OtherUserId &&
                _db.Likes.Any(l2 =>
                    l2.LikerUserId == l1.TargetProfile!.UserId &&
                    l2.TargetProfileId == l1.TargetProfileId),
            ct);
    }
}