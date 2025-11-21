using MediatR;

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

public record DeleteListingCommand(Guid ListingId, Guid UserId) : IRequest<bool>;
