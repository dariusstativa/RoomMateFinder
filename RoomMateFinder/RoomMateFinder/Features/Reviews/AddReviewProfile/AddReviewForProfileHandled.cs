using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.Reviews.AddReviewProfile;

public class AddReviewForProfileHandler : IRequestHandler<AddReviewForProfileCommand, Guid>
{
    private readonly AppDbContext _db;

    public AddReviewForProfileHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(AddReviewForProfileCommand req, CancellationToken ct)
    {
        var profile = await _db.Profiles
            .FirstOrDefaultAsync(x => x.Id == req.ProfileId, ct);

        if (profile == null)
            throw new Exception("Profile not found.");

        var reviewer = await _db.Users
            .FirstOrDefaultAsync(x => x.Id == req.ReviewerId, ct);

        if (reviewer == null)
            throw new Exception("Reviewer not found.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            ReviewerId = req.ReviewerId,
            ProfileId = req.ProfileId,
            Rating = req.Rating,
            Comment = req.Comment
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(ct);

        return review.Id;
    }
}