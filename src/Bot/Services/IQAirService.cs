using IQAirApiClient;
using IQAirApiClient.Models;

namespace Bot.Services;

public class IQAirService
{
    private readonly IQAirApi _iqAirClient;

    private IList<CountryItem>? _countriesList;

    private IList<CountryItem>? CountriesList
    {
        get
        {
            return _countriesList ??= _iqAirClient.ListSupportedCountries().Result;
        }
    }

    public IQAirService(IQAirApi iqAirClient)
    {
        _iqAirClient = iqAirClient;
        _countriesList = null;
    }

    public int GetCountriesCount()
    {
        return CountriesList.Count;
    }

    public IList<CountryItem> GetCountriesPage(int page, int pageSize)
    {
        return CountriesList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}