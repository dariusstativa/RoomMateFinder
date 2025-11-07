using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public class UpdateListingHandler : IRequestHandler<UpdateListingCommand, bool>
{
    private readonly AppDbContext _db;
    public UpdateListingHandler(AppDbContext db) => _db = db;

    public async Task<bool> Handle(UpdateListingCommand request, CancellationToken ct)
    {
        var existing = await _db.RoomListings.FindAsync(new object[] { request.Id }, ct);
        if (existing is null) return false;

        _db.Entry(existing).CurrentValues.SetValues(request.Updated);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}