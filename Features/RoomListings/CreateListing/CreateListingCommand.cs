using MediatR;

namespace RoomMateFinder.Features.RoomListings.CreateListing;

public record CreateRoomListingCommand(Guid OwnerId, CreateListingRequest Request) 
    : IRequest<Guid>;