using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Data.Models;
using AirBro.TelegramBot.Exceptions;
using AirBro.TelegramBot.Models;
using Microsoft.EntityFrameworkCore;

namespace AirBro.TelegramBot.Services;

public class LocationsService
{
    private readonly ApplicationDbContext _db;

    public LocationsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Location?> GetLocationAsync(string country, string state, string city)
    { 
        var countryDb = await _db.Countries.SingleOrDefaultAsync(c => c.Name == country);

        if (countryDb is null)
        {
            throw new LocationNotFoundException();
        }

        return await _db.Locations
            .AsNoTracking()
            .SingleOrDefaultAsync(l => 
                l.CountryId == countryDb.Id && 
                l.State == state && 
                l.City == city);
    }

    public async Task<Location> AddLocationAsync(LocationDto locationDto)
    {
        var countryDb = await _db.Countries.SingleOrDefaultAsync(c => c.Name == locationDto.Country);

        if (countryDb is null)
        {
            throw new LocationNotFoundException();
        }

        var location = new Location
        {
            CountryId = countryDb.Id,
            State = locationDto.State,
            City = locationDto.City,
            Longitude = locationDto.Longitude,
            Latitude = locationDto.Latitude,
        };
        
        _db.Locations.Add(location);
        await _db.SaveChangesAsync();
        
        return location;
    }
}