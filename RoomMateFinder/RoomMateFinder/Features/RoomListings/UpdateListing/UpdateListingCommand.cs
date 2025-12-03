using MediatR;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public record UpdateListingCommand(Guid ListingId, Guid UserId, UpdateListingRequest Request) : IRequest<bool>;
