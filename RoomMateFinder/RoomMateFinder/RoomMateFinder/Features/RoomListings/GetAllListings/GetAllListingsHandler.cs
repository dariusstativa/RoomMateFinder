using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RoomMateFinder.Features.RoomListings.GetAllListings;

public class GetAllListingsHandler : IRequestHandler<GetAllListingsQuery, List<RoomListing>>
{
    private readonly AppDbContext _db;
    public GetAllListingsHandler(AppDbContext db) => _db = db;

    public async Task<List<RoomListing>> Handle(GetAllListingsQuery request, CancellationToken ct)
    {
        return await _db.RoomListings
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }
}