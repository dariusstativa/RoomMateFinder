using MediatR;
using RoomMateFinder.Domain.Entities;
using System.Collections.Generic;

namespace RoomMateFinder.Features.RoomListings.GetAllListings;

public record GetAllListingsQuery() : IRequest<List<RoomListing>>;