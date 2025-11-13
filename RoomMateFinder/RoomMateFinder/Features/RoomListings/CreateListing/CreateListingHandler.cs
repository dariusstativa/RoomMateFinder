using FluentValidation;
using MediatR;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Infrastructure.Persistence;

namespace RoomMateFinder.Features.RoomListings.CreateListing;

public class CreateRoomListingHandler : IRequestHandler<CreateRoomListingCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IValidator<CreateListingRequest> _validator;

    public CreateRoomListingHandler(AppDbContext db, IValidator<CreateListingRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Guid> Handle(CreateRoomListingCommand request, CancellationToken cancellationToken)
    {
        
        var validation = _validator.Validate(request.Request);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var listing = new RoomListing
        {
            Id = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            Title = request.Request.Title,
            Description = request.Request.Description,
            Address = request.Request.Address,
            Price = request.Request.Price,
            RoommatesCount = request.Request.RoommatesCount,
            GenderPreference = request.Request.GenderPreference,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.RoomListings.Add(listing);
        await _db.SaveChangesAsync(cancellationToken);

        return listing.Id;
    }
}