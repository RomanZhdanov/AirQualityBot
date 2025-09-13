using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;

namespace AirBro.TelegramBot.Interfaces;

public interface IAirApiService
{
    event EventHandler ApiLimitReached;
    
    
    Task<AirQualityResult?> GetAir(string country, string state, string city);
    
    Task<AirQualityResult?> GetNearestCityAir(double latitude, double longitude);

    Task<IList<CountryItem>> GetCountries();

    Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize);

    Task<PaginatedList<CityItem>> GetCitiesPage(string country, string state, int page, int pageSize);
}