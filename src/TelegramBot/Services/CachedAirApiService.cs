using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AirBro.TelegramBot.Services;

public class CachedAirApiService : IAirApiService
{
    private readonly IMemoryCache _cache;
    private readonly IAirApiService _airApiService;

    public CachedAirApiService(IMemoryCache cache, IAirApiService airApiService)
    {
        _cache = cache;
        _airApiService = airApiService;
    }

    public async Task<AirQualityResult?> GetAir(string country, string state, string city)
    {
        var key = $"{city}_{state}_{country}";
        var result = _cache.Get<AirQualityResult>(key);

        if (result is null)
        {
            result = await _airApiService.GetAir(country, state, city);
            _cache.Set(key, result, TimeSpan.FromHours(1));
        }
        
        return result;
    }

    public async Task<IList<CountryItem>> GetCountries()
    {
        return await _airApiService.GetCountries();
    }

    public async Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize)
    {
        return await _airApiService.GetStatesPage(country, page, pageSize);
    }

    public async Task<PaginatedList<CityItem>> GetCitiesPage(string country, string state, int page, int pageSize)
    {
        return await _airApiService.GetCitiesPage(country, state, page, pageSize);
    }
}