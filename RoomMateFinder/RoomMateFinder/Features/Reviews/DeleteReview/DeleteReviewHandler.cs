using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Reviews.DeleteReview;

public class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly AppDbContext _db;

    public DeleteReviewHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteReviewCommand req, CancellationToken ct)
    {
        var review = await _db.Reviews
            .FirstOrDefaultAsync(x => x.Id == req.ReviewId, ct);

        if (review == null)
            return false;

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync(ct);

        return true;
    }
}