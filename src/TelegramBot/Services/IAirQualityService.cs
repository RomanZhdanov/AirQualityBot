using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;

namespace AirBro.TelegramBot.Services;

public interface IAirQualityService
{
    Task<AirQualityResult> GetAir(string city, string state, string country);

    Task<IList<CountryItem>> GetCountries();

    Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize);

    Task<PaginatedList<CityItem>?> GetCitiesPage(string country, string state, int page, int pageSize);
}