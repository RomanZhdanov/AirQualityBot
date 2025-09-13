using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Data.Models;
using AirBro.TelegramBot.Exceptions;
using AirBro.TelegramBot.Models.Mappers;
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

    public async Task<int> GetUserLocationsCountAsync(long chatId)
    {
        return await _context.Users.Where(u => u.Id == chatId)
            .Select(u => u.Locations.Count)
            .FirstOrDefaultAsync();    
    }

    public async Task<IReadOnlyCollection<Data.Models.Location>> GetUserLocationsAsync(long chatId)
    {
        var user = await _context.Users
            .Include(u => u.Locations)
            .ThenInclude(l => l.Country)
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
        
        var country = await _context.Countries.SingleOrDefaultAsync(c => c.Name == location.Country);

        if (country is null)
        {
            return;
        }

        var loc = await _context.Locations
            .SingleOrDefaultAsync(l =>
                l.CountryId == country.Id && 
                l.State == location.State && 
                l.City == location.City);

        if (loc is null)
        {
            loc = new Data.Models.Location
            {
                CountryId = country.Id,
                State = location.State,
                City = location.City
            };
            _context.Locations.Add(loc);
        }
        
        user.Locations.Add(loc);
        await _context.SaveChangesAsync();
    }

    public async Task<Location> RemoveUserLocationAsync(long chatId, int locationId)
    {
        var user = await _context.Users
            .AsTracking()
            .Include(u => u.Locations)
            .ThenInclude(l => l.Country)
            .SingleOrDefaultAsync(u => u.Id == chatId);

        if (user is null)
        {
            throw new Exception("User not found");
        }
        
        var location = user.Locations.SingleOrDefault(l => l.Id == locationId);

        if (location is null)
        {
            throw new LocationNotFoundException();
        }
        
        user.Locations.Remove(location);
        await _context.SaveChangesAsync();
        
        return location.ToLocation();
    }
}