using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Reviews.AddReviewListing;

public class AddReviewForListingHandler : IRequestHandler<AddReviewForListingCommand, Guid>
{
    private readonly AppDbContext _db;

    public AddReviewForListingHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(AddReviewForListingCommand req, CancellationToken ct)
    {
        var listing = await _db.RoomListings
            .FirstOrDefaultAsync(x => x.Id == req.ListingId, ct);

        if (listing == null)
            throw new Exception("Listing not found.");

        var reviewer = await _db.Users
            .FirstOrDefaultAsync(x => x.Id == req.ReviewerId, ct);

        if (reviewer == null)
            throw new Exception("Reviewer not found.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            ReviewerId = req.ReviewerId,
            RoomListingId = req.ListingId,
            Rating = req.Rating,
            Comment = req.Comment
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(ct);

        return review.Id;
    }
}