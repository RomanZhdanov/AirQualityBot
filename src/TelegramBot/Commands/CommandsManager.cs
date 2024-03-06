using AirBro.TelegramBot.Commands.SetUserCity;
using AirBro.TelegramBot.Commands.SetUserCountry;
using AirBro.TelegramBot.Commands.SetUserLocation;
using AirBro.TelegramBot.Commands.SetUserState;
using AirBro.TelegramBot.Commands.ShowAir;
using AirBro.TelegramBot.Commands.UpdateCitiesPage;
using AirBro.TelegramBot.Commands.UpdateCountriesPage;
using AirBro.TelegramBot.Commands.UpdateStatesPage;
using AirBro.TelegramBot.Commands.Welcome;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;

namespace AirBro.TelegramBot.Commands;

public class CommandsManager
{
    private readonly IQAirService _airService;
    private readonly Dictionary<long, UserProfile> _usersData;
    
    public CommandsManager(IQAirService airService, Dictionary<long, UserProfile> usersData)
    {
        _airService = airService;
        _usersData = usersData;
    }
    
    public IBotCommandHandler GetCommandHandler(string command)
    {
        return command switch
        {
            "/set_location" => new SetUserLocationCommandHandler(_airService),
            "/show_air" => new ShowAirCommandHandler(_airService, _usersData),
            "set_country" => new SetUserCountryCommandHandler(_airService, _usersData),
            "set_state" => new SetUserStateCommandHandler(_airService, _usersData),
            "set_city" => new SetUserCityCommandHandler(_usersData),
            "countries_page" => new UpdateCountriesPageCommandHandler(_airService),
            "states_page" => new UpdateStatesPageCommandHandler(_airService),
            "cities_page" => new UpdateCitiesPageCommandHandler(_airService),
            _ => new WelcomeCommandHandler()
        };
    }
}