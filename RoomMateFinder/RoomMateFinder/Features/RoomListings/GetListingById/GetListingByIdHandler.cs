using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings.GetListingById;

public class GetListingByIdHandler : IRequestHandler<GetListingByIdQuery, RoomListing?>
{
    private readonly AppDbContext _db;
    public GetListingByIdHandler(AppDbContext db) => _db = db;

    public async Task<RoomListing?> Handle(GetListingByIdQuery request, CancellationToken ct)
    {
        return await _db.RoomListings
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
    }
}