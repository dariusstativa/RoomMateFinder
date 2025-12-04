using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.CreateProfile;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class CreateProfileHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

        return new AppDbContext(options);
    }

    private static CreateProfileRequest ValidRequest =>
        new(
            FullName: "John Doe",
            Age: 21,
            Gender: "Male",
            University: "UAIC",
            Bio: "Some description.",
            SleepSchedule: "Night owl",
            Cleanliness: "Clean",
            NoiseTolerance: "Medium",
            SmokingPreference: "Non-smoker",
            PetPreference: "No pets",
            StudyHabits: "Regular"
        );

    [Fact]
    public async Task Handle_ValidRequest_Creates_Profile_And_Returns_Id()
    {
        
        using var db = CreateDbContext();
        var validator = new CreateProfileValidator();
        var handler = new CreateProfileHandler(db, validator);

        var userId = Guid.NewGuid();
        var command = new CreateProfileCommand(userId, ValidRequest);

        
        var profileId = await handler.Handle(command, CancellationToken.None);

        
        Assert.NotEqual(Guid.Empty, profileId);

        var profile = await db.Profiles.FindAsync(profileId);
        Assert.NotNull(profile);
        Assert.Equal(userId, profile!.UserId);
        Assert.Equal("John Doe", profile.FullName);
        Assert.Equal(21, profile.Age);
        Assert.Equal("Male", profile.Gender);
        Assert.Equal("UAIC", profile.University);
        Assert.False(profile.IsOnboarded);
        Assert.Null(profile.OnboardedAt);
    }

    [Fact]
    public async Task Handle_When_User_Already_Has_Profile_Throws_ValidationException()
    {
        
        using var db = CreateDbContext();
        var validator = new CreateProfileValidator();
        var handler = new CreateProfileHandler(db, validator);

        var userId = Guid.NewGuid();

       
        db.Profiles.Add(new Profile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = "Existing User",
            Age = 22,
            Gender = "M",
            University = "UAIC",
            Bio = "",
            SleepSchedule = "Night owl",
            Cleanliness = "Clean",
            NoiseTolerance = "Low",
            SmokingPreference = "Non-smoker",
            PetPreference = "No pets",
            StudyHabits = "Regular",
            IsOnboarded = true,
            OnboardedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var command = new CreateProfileCommand(userId, ValidRequest);

        
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Invalid_Request_Throws_ValidationException()
    {
        
        using var db = CreateDbContext();
        var validator = new CreateProfileValidator();
        var handler = new CreateProfileHandler(db, validator);

        var invalidRequest = ValidRequest with { FullName = "" }; // invalid
        var command = new CreateProfileCommand(Guid.NewGuid(), invalidRequest);

        
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
