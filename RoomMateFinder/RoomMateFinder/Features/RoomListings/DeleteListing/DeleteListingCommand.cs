using MediatR;

namespace RoomMateFinder.Features.RoomListings.DeleteListing;

public record DeleteListingCommand(Guid Id) : IRequest<bool>;