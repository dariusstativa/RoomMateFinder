using MediatR;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

<<<<<<< HEAD
public record UpdateListingCommand(Guid Id, UpdateListingRequest Request) : IRequest<bool>;
=======
public record UpdateListingCommand(Guid ListingId, Guid UserId, UpdateListingRequest Request) : IRequest<bool>;
>>>>>>> DariusBranch
