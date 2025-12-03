using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.GetAllProfiles;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.Handlers;

public class GetAllProfilesHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_WhenProfilesExist_Returns_All_In_Alphabetical_Order()
    {
        
        using var db = CreateDbContext();

        db.Profiles.AddRange(
            new Profile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                FullName = "Charlie Brown",
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
            },
            new Profile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                FullName = "Alice Johnson",
                Age = 22,
                Gender = "F",
                University = "UAIC",
                Bio = "",
                SleepSchedule = "Early bird",
                Cleanliness = "Medium",
                NoiseTolerance = "High",
                SmokingPreference = "Smoker",
                PetPreference = "Pets allowed",
                StudyHabits = "Intensive",
                IsOnboarded = true,
                OnboardedAt = DateTime.UtcNow
            },
            new Profile
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                FullName = "Bob Smith",
                Age = 23,
                Gender = "M",
                University = "UAIC",
                Bio = "",
                SleepSchedule = "Flexible",
                Cleanliness = "Clean",
                NoiseTolerance = "Medium",
                SmokingPreference = "Non-smoker",
                PetPreference = "No pets",
                StudyHabits = "Regular",
                IsOnboarded = false,
                OnboardedAt = null
            }
        );

        await db.SaveChangesAsync();

        var handler = new GetAllProfilesHandler(db);
        var query = new GetAllProfilesQuery();

       
        var result = await handler.Handle(query, CancellationToken.None);

       
        Assert.Equal(3, result.Count);

      
        var orderedNames = result.Select(p => p.FullName).ToList();
        Assert.Equal(new[] { "Alice Johnson", "Bob Smith", "Charlie Brown" }, orderedNames);
    }

    [Fact]
    public async Task Handle_WhenNoProfilesExist_Returns_EmptyList()
    {
        
        using var db = CreateDbContext();
        var handler = new GetAllProfilesHandler(db);
        var query = new GetAllProfilesQuery();

       
        var result = await handler.Handle(query, CancellationToken.None);

        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
