using MediatR;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings.CreateListing;

public class CreateListingHandler : IRequestHandler<CreateListingCommand, Guid>
{
    private readonly AppDbContext _db;
    public CreateListingHandler(AppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateListingCommand request, CancellationToken ct)
    {
        var listing = request.Listing;
        listing.Id = Guid.NewGuid();
        listing.CreatedAt = DateTime.UtcNow;

        _db.RoomListings.Add(listing);
        await _db.SaveChangesAsync(ct);
        return listing.Id;
    }
}