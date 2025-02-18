using AirBro.TelegramBot.Commands.AddUserLocation;
using AirBro.TelegramBot.Commands.ShowAir;
using AirBro.TelegramBot.Commands.ShowUserLocations;
using AirBro.TelegramBot.Commands.Welcome;
using AirBro.TelegramBot.Services;

namespace AirBro.TelegramBot.Commands;

public class CommandsManager
{
    private readonly IQAirService _airService;
    private readonly UserDataService _userService;
    private readonly TempUserDataService _tempUserData;

    private WelcomeCommandHandler? _welcomeCommandHandler;
    private AddUserLocationCommandHandler? _addUserLocationCommandHandler;
    private ShowAirCommandHandler? _showAirCommandHandler;
    private ShowUserLocationsCommandHandler? _showUserLocationsCommandHandler;
    
    public CommandsManager(IQAirService airService, UserDataService userService, TempUserDataService tempUserData)
    {
        _airService = airService;
        _userService = userService;
        _tempUserData = tempUserData;
    }
    
    public IBotCommandHandler? GetCommandHandler(string command)
    {
        return command switch
        {
            "/example" => null,
            "/add_location" => _addUserLocationCommandHandler ??= new AddUserLocationCommandHandler(_airService, _userService, _tempUserData),
            "/my_locations" => _showUserLocationsCommandHandler ??= new ShowUserLocationsCommandHandler(_userService),
            "/aqi_guide" => null,
            "/settings" => null,
            "/show_air" => _showAirCommandHandler ??= new ShowAirCommandHandler(_airService, _userService),
            _ => _welcomeCommandHandler ??= new WelcomeCommandHandler()
        };
    }
}