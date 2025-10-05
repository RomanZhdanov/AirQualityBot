using System.Collections.Concurrent;
using System.Net;
using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using IQAirApiClient;
using IQAirApiClient.Models;

namespace AirBro.TelegramBot.Services;

public class AirVisualApiService : IAirApiService 
{
    private readonly IAirVisualApi _airVisualClient;

    private IList<CountryItem> _countriesList = [];
    private readonly ConcurrentDictionary<string, IList<StateItem>> _statesDictionary = new();
    private readonly ConcurrentDictionary<string, IList<CityItem>> _citiesDictionary = new();
    
    public event EventHandler ApiLimitReached;
    
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
            LocationDto = new LocationDto(cityData.City, cityData.State, cityData.Country, cityData.Location.Coordinates),
            Aqi = cityData.Current.Pollution.Aqius,
            LastUpdate = cityData.Current.Pollution.Ts
        };
    }

    public async Task<AirQualityResult?> GetNearestCityAir(double latitude, double longitude)
    {
        var cityData = await ExecuteWithRetryAsync(async () =>
            await _airVisualClient.GetNearestCityData(latitude, longitude));
        
        if (cityData is null) return null;

        return new AirQualityResult
        {
            LocationDto = new LocationDto(cityData.City, cityData.State, cityData.Country, cityData.Location.Coordinates),
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
        int seconds = 60;

        while (retries < maxRetries)
        {
            var result = await operation();

            if (result.StatusCode == HttpStatusCode.TooManyRequests)
            {
                ApiLimitReached?.Invoke(this, EventArgs.Empty);
                retries++;
                await Task.Delay(TimeSpan.FromSeconds(seconds));
                seconds *= 2;
                continue;
            }

            return result.Data;
        }

        return null;
    }
}