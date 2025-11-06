using FluentValidation;
using MediatR;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Infrastructure.Persistence;

public record CreateProfileCommand(Guid UserId, CreateProfileRequest Request) : IRequest<Guid>;

public class CreateProfileHandler : IRequestHandler<CreateProfileCommand, Guid>
{
    private readonly AppDbContext _db;
    private readonly IValidator<CreateProfileRequest> _validator;

    public CreateProfileHandler(AppDbContext db, IValidator<CreateProfileRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<Guid> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        
        await _validator.ValidateAndThrowAsync(request.Request, cancellationToken);

        var p = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FullName = request.Request.FullName,
            Age = request.Request.Age,
            Gender = request.Request.Gender,
            University = request.Request.University,
            Bio = request.Request.Bio,
            SleepSchedule = request.Request.SleepSchedule,
            Cleanliness = request.Request.Cleanliness,
            NoiseTolerance = request.Request.NoiseTolerance,
            SmokingPreference = request.Request.SmokingPreference,
            PetPreference = request.Request.PetPreference,
            StudyHabits = request.Request.StudyHabits
        };

        _db.Profiles.Add(p);
        await _db.SaveChangesAsync(cancellationToken);
        return p.Id;
    }
}