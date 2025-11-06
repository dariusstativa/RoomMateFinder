using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RoomMateFinder.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
       
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

       
        const string connectionString =
            "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=sirene99";

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}