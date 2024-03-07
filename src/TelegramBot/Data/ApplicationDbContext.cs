using AirBro.TelegramBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AirBro.TelegramBot.Data;

public class ApplicationDbContext : DbContext
{
    // public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<UserLocation> UsersLocations => Set<UserLocation>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=/home/roman/Data/AirBroBotDb.db");
    }
}