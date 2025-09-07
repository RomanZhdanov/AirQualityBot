using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;

namespace AirBro.TelegramBot.Interfaces;

public interface IAirApiService
{
    Task<AirQualityResult?> GetAir(string country, string state, string city);

    Task<IList<CountryItem>> GetCountries();

    Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize);

    Task<PaginatedList<CityItem>> GetCitiesPage(string country, string state, int page, int pageSize);
}