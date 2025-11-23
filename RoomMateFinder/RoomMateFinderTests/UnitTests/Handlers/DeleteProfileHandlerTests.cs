using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.DeleteProfile;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class DeleteProfileHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_ProfileExists_ReturnsTrue_And_DeletesProfile()
    {
        
        using var db = CreateDbContext();
        var handler = new DeleteProfileHandler(db);

        var userId = Guid.NewGuid();

       
        var profile = new Profile
        {
            Id = userId,          
            UserId = userId,      
            FullName = "John Doe",
            Age = 21,
            Gender = "M",
            University = "UAIC",
            Bio = "",
            SleepSchedule = "Night owl",
            Cleanliness = "Clean",
            NoiseTolerance = "Low",
            SmokingPreference = "Non-smoker",
            PetPreference = "No pets",
            StudyHabits = "Regular",
            IsOnboarded = false,
            OnboardedAt = null
        };

        db.Profiles.Add(profile);
        await db.SaveChangesAsync();

        var command = new DeleteProfileCommand(userId);

       
        var result = await handler.Handle(command, CancellationToken.None);

      
        Assert.True(result);

        var deleted = await db.Profiles.FindAsync(userId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Handle_ProfileDoesNotExist_ReturnsFalse()
    {
        
        using var db = CreateDbContext();
        var handler = new DeleteProfileHandler(db);

        var command = new DeleteProfileCommand(Guid.NewGuid());

       
        var result = await handler.Handle(command, CancellationToken.None);

        
        Assert.False(result);
    }
}
