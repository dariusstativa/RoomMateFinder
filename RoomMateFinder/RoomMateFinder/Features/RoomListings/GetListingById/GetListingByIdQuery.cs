using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.RoomListings.GetListingById;

public record GetListingByIdQuery(Guid Id) : IRequest<RoomListing?>;