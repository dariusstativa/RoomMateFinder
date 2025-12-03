using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RoomMateFinder.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        const string connectionString =
<<<<<<< HEAD
            "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=3924";
=======
            "Host=localhost;Port=5432;Database=roommatefinder;Username=postgres;Password=sirene99";
>>>>>>> DariusBranch

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}