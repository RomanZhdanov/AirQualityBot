using System.Collections.Concurrent;
using System.Net;
using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using IQAirApiClient;
using IQAirApiClient.Models;
using Location = AirBro.TelegramBot.Models.Location;

namespace AirBro.TelegramBot.Services;

public class AirVisualApiService : IAirApiService 
{
    private readonly IAirVisualApi _airVisualClient;

    private IList<CountryItem> _countriesList = [];
    private readonly ConcurrentDictionary<string, IList<StateItem>> _statesDictionary = new();
    private readonly ConcurrentDictionary<string, IList<CityItem>> _citiesDictionary = new();
    
    public AirVisualApiService(IAirVisualApi airVisualClient)
    {
        _airVisualClient = airVisualClient;
    }
    
    public async Task<AirQualityResult?> GetAir(string country, string state, string city)
    {
        var cityData = await ExecuteWithRetryAsync(async () => 
            await _airVisualClient.GetSpecifiedCityData(country, state, city));

        if (cityData is null) return null;
        
        return new AirQualityResult()
        {
            Location = new Location(cityData.City, cityData.State, cityData.Country),
            Aqi = cityData.Current.Pollution.Aqius,
            LastUpdate = cityData.Current.Pollution.Ts
        };
    }

    public async Task<IList<CountryItem>> GetCountries()
    {
        if (_countriesList.Count == 0)
        {
            var result = await ExecuteWithRetryAsync(async () =>
                await _airVisualClient.ListSupportedCountries());

            if (result is not null)
            {
                _countriesList = result;
            }
        }
        
        return _countriesList;
    }
    
    private async Task<IList<StateItem>> GetStates(string country)
    {
        IList<StateItem> states = [];
        
        if (_statesDictionary.ContainsKey(country))
        {
            states = _statesDictionary[country];
        }
        else
        {
            var result = await ExecuteWithRetryAsync(async () =>
                await _airVisualClient.ListSupportedStatesInCountry(country));

            if (result is not null)
            {
                states = result;
            }
            
            _statesDictionary.TryAdd(country, states);
        }

        return states;
    }

    public async Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize)
    {
        var states = await GetStates(country);

        var statesPage = states
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();

        return new PaginatedList<StateItem>(statesPage, states.Count, page, pageSize);
    }

    public async Task<PaginatedList<CityItem>> GetCitiesPage(string country, string state, int page, int pageSize)
    {
        var cities = await GetCities(country, state);

        var citiesPage = cities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();

        return new PaginatedList<CityItem>(citiesPage, cities.Count, page, pageSize);
    }

    private async Task<IList<CityItem>> GetCities(string country, string state)
    {
        IList<CityItem> cities = [];
        var key = $"{country}|{state}";
        
        if (_citiesDictionary.ContainsKey(key))
        {
            cities = _citiesDictionary[key];
        }
        else
        {
            var result = await ExecuteWithRetryAsync(async () =>
                await _airVisualClient.ListSupportedCitiesInState(country, state));

            if (result is not null)
            {
                cities = result;
            }
            
            _citiesDictionary.TryAdd(key, cities);
        }

        return cities;
    }

    private async Task<T?> ExecuteWithRetryAsync<T>(Func<Task<Result<T>>> operation)
        where T : class
    {
        const int maxRetries = 3;
        int retries = 0;

        while (retries < maxRetries)
        {
            var result = await operation();

            if (result.StatusCode == HttpStatusCode.TooManyRequests)
            {
                retries++;
                await Task.Delay(TimeSpan.FromSeconds(15));
                continue;
            }

            return result.Data;
        }

        return null;
    }
}