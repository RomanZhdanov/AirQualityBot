using AirBro.TelegramBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AirBro.TelegramBot.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    
    public DbSet<Country> Countries => Set<Country>();
    
    public DbSet<Location> Locations => Set<Location>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(e => e.Locations)
            .WithMany(e => e.Users)
            .UsingEntity(
                "UserLocation",
                l => l.HasOne(typeof(Location)).WithMany().HasForeignKey("LocationId").HasPrincipalKey(nameof(Location.Id)),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").HasPrincipalKey(nameof(User.Id)),
                j => j.HasKey("UserId", "LocationId"));;
    }
}