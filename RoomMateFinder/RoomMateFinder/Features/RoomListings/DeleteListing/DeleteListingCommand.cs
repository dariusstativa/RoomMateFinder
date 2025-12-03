using MediatR;

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

<<<<<<< HEAD
public record DeleteListingCommand(Guid Id) : IRequest<bool>;
=======
public record DeleteListingCommand(Guid ListingId, Guid UserId) : IRequest<bool>;
>>>>>>> DariusBranch
