using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;
using RoomMateFinder.Features.Profiles.CompleteOnboarding;
using RoomMateFinder.Infrastructure.Persistence;
using Xunit;

namespace RoomMateFinderTests.UnitTests.new_tests;

public class CompleteOnboardingHandlerTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_ProfileExists_SetsOnboarded()
    {
        await using var db = CreateDb();

        var userId = Guid.NewGuid();

        db.Profiles.Add(new Profile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsOnboarded = false
        });

        await db.SaveChangesAsync();

        var handler = new CompleteOnboardingHandler(db);
        var cmd = new CompleteOnboardingCommand(
            userId,
            new CompleteOnboardingRequest()
        );

        var result = await handler.Handle(cmd, CancellationToken.None);

        var profile = await db.Profiles.FirstAsync();

        Assert.True(result);
        Assert.True(profile.IsOnboarded);
        Assert.NotNull(profile.OnboardedAt);
    }

    [Fact]
    public async Task Handle_ProfileMissing_ReturnsFalse()
    {
        await using var db = CreateDb();

        var handler = new CompleteOnboardingHandler(db);
        var cmd = new CompleteOnboardingCommand(
            Guid.NewGuid(),
            new CompleteOnboardingRequest()
        );

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.False(result);
    }
}