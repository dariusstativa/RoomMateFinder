using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.RoomListings.CreateListing;

public record CreateListingCommand(RoomListing Listing) : IRequest<Guid>;