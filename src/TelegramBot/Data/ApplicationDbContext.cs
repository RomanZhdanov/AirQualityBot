using AirBro.TelegramBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AirBro.TelegramBot.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<UserLocation> UsersLocations => Set<UserLocation>();
}