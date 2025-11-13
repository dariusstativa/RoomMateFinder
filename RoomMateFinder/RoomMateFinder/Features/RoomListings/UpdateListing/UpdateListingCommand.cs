using MediatR;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public record UpdateListingCommand(Guid Id, UpdateListingRequest Request) : IRequest<bool>;