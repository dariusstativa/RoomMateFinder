using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Reviews.GetReviwesProfile;

public class GetReviewsForProfileHandler
    : IRequestHandler<GetReviewsForProfileQuery, List<ReviewDto>>
{
    private readonly AppDbContext _db;

    public GetReviewsForProfileHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReviewDto>> Handle(GetReviewsForProfileQuery req, CancellationToken ct)
    {
        return await _db.Reviews
            .Where(x => x.ProfileId == req.ProfileId)
            .Include(x => x.Reviewer)
            .ThenInclude(u => u.Profile)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                ReviewerId = x.ReviewerId,
                ReviewerName = x.Reviewer.Profile != null
                    ? x.Reviewer.Profile.FullName
                    : x.Reviewer.Email,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}