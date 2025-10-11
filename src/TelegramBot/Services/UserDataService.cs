using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Data.Models;
using AirBro.TelegramBot.Exceptions;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Models.Mappers;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Location> AddUserLocationAsync(long chatid, int locationId)
    {
        var user = await _context.Users
            .AsTracking()
            .Include(u => u.Locations)
            .SingleOrDefaultAsync(u => u.Id == chatid);

        if (user is null)
        {
            user = new User { Id = chatid };
            _context.Users.Add(user);
        }
        
        var location = await _context.Locations
            .AsTracking()
            .Include(l => l.Country)
            .SingleOrDefaultAsync(l => l.Id == locationId);
        
        if (location is null)
            throw new LocationNotFoundException();
        
        if (user.Locations.Count(l => l.Id == location.Id) > 0)
        {
            throw new LocationAlreadyAddedException();
        }
        
        user.Locations.Add(location);
        await _context.SaveChangesAsync();
        
        return location;   
    }

    public async Task<LocationDto> RemoveUserLocationAsync(long chatId, int locationId)
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
        
        return location.ToLocationDto();
    }
}