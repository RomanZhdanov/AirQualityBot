using AirBro.TelegramBot.Commands.SetUserCity;
using AirBro.TelegramBot.Commands.SetUserCountry;
using AirBro.TelegramBot.Commands.SetUserLocation;
using AirBro.TelegramBot.Commands.SetUserState;
using AirBro.TelegramBot.Commands.ShowAir;
using AirBro.TelegramBot.Commands.UpdateCitiesPage;
using AirBro.TelegramBot.Commands.UpdateCountriesPage;
using AirBro.TelegramBot.Commands.UpdateStatesPage;
using AirBro.TelegramBot.Commands.Welcome;
using AirBro.TelegramBot.Services;

namespace AirBro.TelegramBot.Commands;

public class CommandsManager
{
    private readonly IQAirService _airService;
    private readonly UserDataService _usersData;

    private WelcomeCommandHandler? _welcomeCommandHandler;
    private SetUserLocationCommandHandler? _setUserLocationCommandHandler;
    private ShowAirCommandHandler? _showAirCommandHandler;
    private SetUserCountryCommandHandler? _setUserCountryCommandHandler;
    private SetUserStateCommandHandler? _setUserStateCommandHandler;
    private SetUserCityCommandHandler? _setUserCityCommandHandler;
    private UpdateCountriesPageCommandHandler? _updateCountriesPageCommandHandler;
    private UpdateStatesPageCommandHandler? _updateStatesPageCommandHandler;
    private UpdateCitiesPageCommandHandler? _updateCitiesPageCommandHandler;
    
    public CommandsManager(IQAirService airService, UserDataService usersData)
    {
        _airService = airService;
        _usersData = usersData;
    }
    
    public IBotCommandHandler GetCommandHandler(string command)
    {
        return command switch
        {
            "/example" => null,
            "/add_location" => null,
            "/my_locations" => null,
            "/aqi_guide" => null,
            "/settings" => null,
            "/set_location" => _setUserLocationCommandHandler ??= new SetUserLocationCommandHandler(_airService),
            "/show_air" => _showAirCommandHandler ??= new ShowAirCommandHandler(_airService, _usersData),
            "set_country" => _setUserCountryCommandHandler ??= new SetUserCountryCommandHandler(_airService, _usersData),
            "set_state" => _setUserStateCommandHandler ??= new SetUserStateCommandHandler(_airService, _usersData),
            "set_city" => _setUserCityCommandHandler ??= new SetUserCityCommandHandler(_usersData),
            "countries_page" => _updateCountriesPageCommandHandler ??= new UpdateCountriesPageCommandHandler(_airService),
            "states_page" => _updateStatesPageCommandHandler ??= new UpdateStatesPageCommandHandler(_airService),
            "cities_page" => _updateCitiesPageCommandHandler ??= new UpdateCitiesPageCommandHandler(_airService),
            _ => _welcomeCommandHandler ??= new WelcomeCommandHandler()
        };
    }
}