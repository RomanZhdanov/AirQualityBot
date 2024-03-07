using AirBro.TelegramBot.Commands.AddUserLocation;
using AirBro.TelegramBot.Commands.ShowAir;
using AirBro.TelegramBot.Commands.Welcome;
using AirBro.TelegramBot.Services;

namespace AirBro.TelegramBot.Commands;

public class CommandsManager
{
    private readonly IQAirService _airService;
    private readonly UserDataService _usersData;

    private WelcomeCommandHandler? _welcomeCommandHandler;
    private AddUserLocationCommandHandler? _addUserLocationCommandHandler;
    private ShowAirCommandHandler? _showAirCommandHandler;
    
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
            "/add_location" => _addUserLocationCommandHandler ??= new AddUserLocationCommandHandler(_airService, _usersData),
            "/my_locations" => null,
            "/aqi_guide" => null,
            "/settings" => null,
            "/show_air" => _showAirCommandHandler ??= new ShowAirCommandHandler(_airService, _usersData),
            _ => _welcomeCommandHandler ??= new WelcomeCommandHandler()
        };
    }
}