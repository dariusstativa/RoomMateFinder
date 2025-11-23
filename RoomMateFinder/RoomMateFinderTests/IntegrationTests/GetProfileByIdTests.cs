using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles;
using RoomMateFinder.Features.Profiles.GetProfileById;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class GetProfileByIdHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_WhenProfileExists_Returns_ProfileDto()
    {
        using var db = CreateDbContext();

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Email = "user@example.com",
            PasswordHash = "hash",
            Salt = "salt",
            Role = "Student"
        };

        var profileId = Guid.NewGuid();

        var profile = new Profile
        {
            Id = profileId,
            UserId = userId,
            User = user,
            FullName = "John Doe",
            Age = 21,
            Gender = "M",
            University = "UAIC",
            Bio = "Some bio",
            SleepSchedule = "Night owl",
            Cleanliness = "Clean",
            NoiseTolerance = "Low",
            SmokingPreference = "Non-smoker",
            PetPreference = "No pets",
            StudyHabits = "Regular",
            IsOnboarded = true,
            OnboardedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();

        var handler = new GetProfileByIdHandler(db);
        var query = new GetProfileByIdQuery(profileId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(profileId, result!.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal(21, result.Age);
        Assert.Equal("M", result.Gender);
        Assert.Equal("UAIC", result.University);
        Assert.Equal("Some bio", result.Bio);
        Assert.Equal("Night owl", result.SleepSchedule);
        Assert.Equal("Clean", result.Cleanliness);
        Assert.Equal("Low", result.NoiseTolerance);
        Assert.Equal("Non-smoker", result.SmokingPreference);
        Assert.Equal("No pets", result.PetPreference);
        Assert.Equal("Regular", result.StudyHabits);
    }

    [Fact]
    public async Task Handle_WhenNoProfile_Returns_Null()
    {
        using var db = CreateDbContext();
        var handler = new GetProfileByIdHandler(db);

        var query = new GetProfileByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}
