using Microsoft.EntityFrameworkCore;
using RoomMateFinder.Domain.Entities;

namespace RoomMateFinder.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<RoomListing> RoomListings => Set<RoomListing>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Review> Reviews => Set<Review>();

    // Nou:
    public DbSet<Like> Likes => Set<Like>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany<RoomListing>()
            .WithOne(r => r.Owner)
            .HasForeignKey(r => r.OwnerId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.TargetUser)
            .WithMany()
            .HasForeignKey(r => r.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- ADD LIKE RELATIONSHIPS ----
        modelBuilder.Entity<Like>()
            .HasOne(l => l.LikerUser)
            .WithMany()
            .HasForeignKey(l => l.LikerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.TargetProfile)
            .WithMany()
            .HasForeignKey(l => l.TargetProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
