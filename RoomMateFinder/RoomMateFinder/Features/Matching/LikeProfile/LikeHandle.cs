namespace RoomMateFinder.Features.Matching.LikeProfile;

using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;


public class LikeHandler : IRequestHandler<LikeCommand, bool>
{
    private readonly AppDbContext _db;

    public LikeHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(LikeCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        // Există deja în DB?
        var existing = await _db.Likes.FirstOrDefaultAsync(
            x => x.LikerUserId == req.LikerUserId && x.TargetProfileId == req.TargetProfileId,
            cancellationToken
        );

        if (existing != null)
        {
            // dacă exista dislike, îl transformăm în like
            existing.IsLike = true;
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Nu există → creăm un like nou
        var like = new Like
        {
            LikerUserId = req.LikerUserId,
            TargetProfileId = req.TargetProfileId,
            IsLike = true
        };

        _db.Likes.Add(like);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
