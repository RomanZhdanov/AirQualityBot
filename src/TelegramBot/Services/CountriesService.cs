using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;
using Microsoft.EntityFrameworkCore;

namespace AirBro.TelegramBot.Services;

public class CountriesService
{
    private readonly ApplicationDbContext _db;

    public CountriesService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedList<CountryItem>> GetCountriesPage(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount  = await _db.Countries.CountAsync();
        var countries = await _db.Countries
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CountryItem
            {
                Country = c.Name
            })
            .ToListAsync();
        
        return new PaginatedList<CountryItem>(countries, totalCount, page, pageSize);
    }
}