using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

public class DeleteListingHandler : IRequestHandler<DeleteListingCommand, bool>
{
    private readonly AppDbContext _db;
    public DeleteListingHandler(AppDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteListingCommand request, CancellationToken ct)
    {
        var listing = await _db.RoomListings
            .FirstOrDefaultAsync(x => x.Id == request.ListingId && x.OwnerId == request.UserId, ct);

        if (listing is null)
            return false;

        _db.RoomListings.Remove(listing);
        await _db.SaveChangesAsync(ct);

        return true;
    }
}