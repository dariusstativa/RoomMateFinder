using MediatR;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public record UpdateListingCommand(Guid Id, RoomListing Updated) : IRequest<bool>;