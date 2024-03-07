using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AirBro.TelegramBot.Services;

public class UserDataService
{
    private readonly ApplicationDbContext _context;

    public UserDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserLocation> GetUserLocationAsync(long chatId)
    {
        try
        {
            var userLocation = await _context.UsersLocations.SingleOrDefaultAsync(u => u.ChatId == chatId);

            if (userLocation is null)
            {
                userLocation = new UserLocation(chatId);
                _context.Add(userLocation);
            }

            return userLocation;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task SaveUserLocationCityAsync(long chatId, string city)
    {
        var userLocation = await GetUserLocationAsync(chatId);

        userLocation.City = city;

        await _context.SaveChangesAsync();
    }
    
    public async Task SaveUserLocationStateAsync(long chatId, string state)
    {
        var userLocation = await GetUserLocationAsync(chatId);

        userLocation.State = state;

        await _context.SaveChangesAsync();
    }
    
    public async Task SaveUserLocationCountryAsync(long chatId, string country)
    {
        var userLocation = await GetUserLocationAsync(chatId);

        userLocation.Country = country;

        await _context.SaveChangesAsync();
    }
}