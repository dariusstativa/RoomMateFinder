using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.UpdateProfile;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class UpdateProfileHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    
    private static UpdateProfileRequest CreateValidRequest()
        => new(
            SleepSchedule: "Night owl",
            Cleanliness: "Clean",
            NoiseTolerance: "Low",
            SmokingPreference: "Non-smoker",
            PetPreference: "No pets",
            StudyHabits: "Regular",
            Bio: "Updated bio"
        );

    [Fact]
    public async Task Handle_ValidRequest_And_ProfileExists_Updates_Profile_And_Returns_True()
    {
       
        using var db = CreateDbContext();

        var userId = Guid.NewGuid();

        var existingProfile = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = "John Doe",
            Age = 21,
            Gender = "M",
            University = "UAIC",
            Bio = "Old bio",
            SleepSchedule = "Early bird",
            Cleanliness = "Messy",
            NoiseTolerance = "High",
            SmokingPreference = "Smoker",
            PetPreference = "Pets allowed",
            StudyHabits = "Chaotic",
            IsOnboarded = false,
            OnboardedAt = null
        };

        db.Profiles.Add(existingProfile);
        await db.SaveChangesAsync();

        var validator = new UpdateProfileValidator();
        var handler = new UpdateProfileHandler(db, validator);

        var request = CreateValidRequest();
        var command = new UpdateProfileCommand(userId, request);

       
        var result = await handler.Handle(command, CancellationToken.None);

        
        Assert.True(result);

        var updated = await db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        Assert.NotNull(updated);

        Assert.Equal("Updated bio", updated!.Bio);
        Assert.Equal("Night owl", updated.SleepSchedule);
        Assert.Equal("Clean", updated.Cleanliness);
        Assert.Equal("Low", updated.NoiseTolerance);
        Assert.Equal("Non-smoker", updated.SmokingPreference);
        Assert.Equal("No pets", updated.PetPreference);
        Assert.Equal("Regular", updated.StudyHabits);
    }

    [Fact]
    public async Task Handle_ProfileDoesNotExist_Returns_False()
    {
       
        using var db = CreateDbContext();

        var validator = new UpdateProfileValidator();
        var handler = new UpdateProfileHandler(db, validator);

        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            Request: CreateValidRequest()
        );

       
        var result = await handler.Handle(command, CancellationToken.None);

       
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_InvalidRequest_Throws_ValidationException()
    {
       
        using var db = CreateDbContext();

        var validator = new UpdateProfileValidator();
        var handler = new UpdateProfileHandler(db, validator);

       
        var invalidRequest = new UpdateProfileRequest(
            SleepSchedule: "",
            Cleanliness: "Clean",
            NoiseTolerance: "Low",
            SmokingPreference: "Non-smoker",
            PetPreference: "No pets",
            StudyHabits: "Regular",
            Bio: "Something"
        );

        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            Request: invalidRequest
        );

        
        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
