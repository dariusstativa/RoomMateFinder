using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RoomMateFinder.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
       
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

       
        const string connectionString =
<<<<<<< Updated upstream
            "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=AICIPAROLAVOASTRA";
=======
            "Host=localhost;Database=RoomMateFinder;Username=postgres;Password=3924";
>>>>>>> Stashed changes

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}