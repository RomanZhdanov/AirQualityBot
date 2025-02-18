using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Data.Models;
using Microsoft.EntityFrameworkCore;
using Location = AirBro.TelegramBot.Models.Location;

namespace AirBro.TelegramBot.Services;

public class UserDataService
{
    private readonly ApplicationDbContext _context;

    public UserDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Data.Models.Location>> GetUserLocationsAsync(long chatId)
    {
        var user = await _context.Users
            .Include(u => u.Locations)
            .SingleOrDefaultAsync(u => u.Id == chatId);

        if (user is null)
        {
            return new List<Data.Models.Location>();
        }

        return user.Locations.OrderBy(l => l.City).ToList();
    }

    public async Task AddUserLocationAsync(long chatid, Location location)
    {
        var user = await _context.Users.FindAsync(chatid);

        if (user is null)
        {
            user = new User { Id = chatid };
            _context.Users.Add(user);
        }

        var loc = await _context.Locations
            .SingleOrDefaultAsync(l =>
                l.Country == location.Country && 
                l.State == location.State && 
                l.City == location.City);

        if (loc is null)
        {
            loc = new Data.Models.Location
            {
                Country = location.Country,
                State = location.State,
                City = location.City
            };
            _context.Locations.Add(loc);
        }
        
        user.Locations.Add(loc);
        await _context.SaveChangesAsync();
    }
}