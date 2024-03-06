using AirBro.TelegramBot.Models;
using IQAirApiClient;
using IQAirApiClient.Models;
using Location = AirBro.TelegramBot.Models.Location;

namespace AirBro.TelegramBot.Services;

public class IQAirService
{
    private readonly IQAirApi _iqAirClient;

    private IList<CountryItem>? _countriesList;
    private Dictionary<string, IList<StateItem>> _statesDictionary = new();
    private Dictionary<string, IList<CityItem>> _citiesDictionary = new();

    private IList<CountryItem>? CountriesList
    {
        get
        {
            return _countriesList ??= _iqAirClient.ListSupportedCountries().Result;
        }
    }
    
    public async Task<AirQualityResult> GetAirForCity(string city, string state, string country)
    {
        var cityData = await _iqAirClient.GetSpecifiedCityData(city, state, country);

        return new AirQualityResult()
        {
            Location = new Location(cityData.City, cityData.State, cityData.Country),
            Aqi = cityData.Current.Pollution.Aqius,
            LastUpdate = cityData.Current.Pollution.Ts
        };
    }

    public IQAirService(IQAirApi iqAirClient)
    {
        _iqAirClient = iqAirClient;
        _countriesList = null;
    }

    public PaginatedList<CountryItem> GetCountriesPage(int page, int pageSize)
    {
        var countries =  CountriesList?
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();

        return new PaginatedList<CountryItem>(countries, CountriesList.Count, page, pageSize);
    }
    
    private async Task<IList<StateItem>> GetStatesForCountry(string country)
    {
        IList<StateItem> states;
        
        if (_statesDictionary.ContainsKey(country))
        {
            states = _statesDictionary[country];
        }
        else
        {
            states = await _iqAirClient.ListSupportedStatesInCountry(country);
            _statesDictionary.Add(country, states);
        }

        return states;
    }

    public async Task<PaginatedList<StateItem>> GetStatesPage(string country, int page, int pageSize)
    {
        var states = await GetStatesForCountry(country);
        
        var statesPage = states?
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();

        return new PaginatedList<StateItem>(statesPage, states.Count, page, pageSize);
    }

    public async Task<PaginatedList<CityItem>> GetCitiesPage(string country, string state, int page, int pageSize)
    {
        var cities = await GetCities(country, state);

        var citiesPage = cities?
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList().AsReadOnly();

        return new PaginatedList<CityItem>(citiesPage, cities.Count, page, pageSize);
    }

    private async Task<IList<CityItem>> GetCities(string country, string state)
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
            _citiesDictionary.Add(key, cities);
        }

        return cities;
    }
}