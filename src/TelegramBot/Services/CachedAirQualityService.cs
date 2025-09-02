using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AirBro.TelegramBot.Services;

public class CachedAirQualityService : IAirQualityService
{
    private readonly IMemoryCache _cache;
    private readonly IAirQualityService _airQualityService;

    public CachedAirQualityService(IMemoryCache cache, IAirQualityService airQualityService)
    {
        _cache = cache;
        _airQualityService = airQualityService;
    }

    public async Task<AirQualityResult> GetAir(string city, string state, string country)
    {
        var key = $"{city}_{state}_{country}";
        var result = _cache.Get<AirQualityResult>(key);

        if (result is null)
        {
            result = await _airQualityService.GetAir(city, state, country);
            _cache.Set(key, result, TimeSpan.FromHours(1));
        }
        
        return result;
    }

    public async Task<IList<CountryItem>> GetCountries()
    {
        return await _airQualityService.GetCountries();
    }

    public async Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize)
    {
        return await _airQualityService.GetStatesPage(country, page, pageSize);
    }

    public async Task<PaginatedList<CityItem>> GetCitiesPage(string country, string state, int page, int pageSize)
    {
        return await _airQualityService.GetCitiesPage(country, state, page, pageSize);
    }
}