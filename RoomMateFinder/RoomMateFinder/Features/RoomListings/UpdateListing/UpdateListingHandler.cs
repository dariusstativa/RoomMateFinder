using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.Results;

namespace RoomMateFinder.Features.RoomListings.UpdateListing;

public class UpdateListingHandler : IRequestHandler<UpdateListingCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly IValidator<UpdateListingRequest> _validator;

    public UpdateListingHandler(AppDbContext db, IValidator<UpdateListingRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<bool> Handle(UpdateListingCommand request, CancellationToken ct)
    {

        ValidationResult validationResult = _validator.Validate(request.Request);
        if (!validationResult.IsValid)
        {
            throw new  ValidationException(validationResult.Errors);
        }
        
        var listing = await _db.RoomListings
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (listing is null)
            return false;

        
        listing.Title = request.Request.Title;
        listing.Description = request.Request.Description;
        listing.Address = request.Request.Address;
        listing.Price = request.Request.Price;
        listing.IsAvailable = request.Request.IsAvailable;
        listing.RoommatesCount = request.Request.RoommatesCount;
        listing.GenderPreference = request.Request.GenderPreference;

        await _db.SaveChangesAsync(ct);
        return true;
    }
}