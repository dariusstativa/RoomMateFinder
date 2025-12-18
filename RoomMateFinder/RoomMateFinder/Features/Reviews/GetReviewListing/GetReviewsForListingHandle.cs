using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Reviews.GetReviewListing;

public class GetReviewsForListingHandler
    : IRequestHandler<GetReviewsForListingQuery, List<ReviewDto>>
{
    private readonly AppDbContext _db;

    public GetReviewsForListingHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReviewDto>> Handle(GetReviewsForListingQuery req, CancellationToken ct)
    {
        return await _db.Reviews
            .Where(x => x.RoomListingId == req.ListingId)
            .Include(x => x.Reviewer)
            .ThenInclude(u => u.Profile)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                ReviewerId = x.ReviewerId,
                ReviewerName = x.Reviewer.Profile != null
                    ? x.Reviewer.Profile.FullName
                    : x.Reviewer.Email, // fallback dacă nu are încă profil
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);

    }
}