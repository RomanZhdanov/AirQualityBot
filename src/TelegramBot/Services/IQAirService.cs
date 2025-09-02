using System.Collections.Concurrent;
using AirBro.TelegramBot.Models;
using IQAirApiClient;
using IQAirApiClient.Models;
using Location = AirBro.TelegramBot.Models.Location;

namespace AirBro.TelegramBot.Services;

public class IQAirService : IAirQualityService 
{
    private readonly IQAirApi _iqAirClient;

    private IList<CountryItem>? _countriesList = null;
    private readonly ConcurrentDictionary<string, IList<StateItem>> _statesDictionary = new();
    private readonly ConcurrentDictionary<string, IList<CityItem>> _citiesDictionary = new();
    
    public IQAirService(IQAirApi iqAirClient)
    {
        _iqAirClient = iqAirClient;
    }
    
    public async Task<AirQualityResult> GetAir(string city, string state, string country)
    {
        var cityData = await _iqAirClient.GetSpecifiedCityData(city, state, country);

        return new AirQualityResult()
        {
            Location = new Location(cityData.City, cityData.State, cityData.Country),
            Aqi = cityData.Current.Pollution.Aqius,
            LastUpdate = cityData.Current.Pollution.Ts
        };
    }

    public async Task<IList<CountryItem>> GetCountries()
    {
        if (_countriesList is null)
        {
            _countriesList = await _iqAirClient.ListSupportedCountries();
        }
        
        return _countriesList;
    }
    
    private async Task<IList<StateItem>> GetStates(string country)
    {
        IList<StateItem> states;
        
        if (_statesDictionary.ContainsKey(country))
        {
            states = _statesDictionary[country];
        }
        else
        {
            states = await _iqAirClient.ListSupportedStatesInCountry(country);
            _statesDictionary.TryAdd(country, states);
        }

        return states;
    }

    public async Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize)
    {
        var states = await GetStates(country);
        
        var statesPage = states?
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();

        return new PaginatedList<StateItem>(statesPage, states.Count, page, pageSize);
    }

    public async Task<PaginatedList<CityItem>?> GetCitiesPage(string country, string state, int page, int pageSize)
    {
        var cities = await GetCities(country, state);

        if (cities is null) return null;

        var citiesPage = cities?
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();

        return new PaginatedList<CityItem>(citiesPage, cities.Count, page, pageSize);
    }

    private async Task<IList<CityItem>?> GetCities(string country, string state)
    {
        IList<CityItem> cities;
        var key = $"{country}|{state}";
        
        if (_citiesDictionary.ContainsKey(key))
        {
            cities = _citiesDictionary[key];
        }
        else
        {
            cities = await _iqAirClient.ListSupportedCitiesInState(country, state);
            if (cities is not null)
            {
                _citiesDictionary.TryAdd(key, cities);
            }
        }

        return cities;
    }
}