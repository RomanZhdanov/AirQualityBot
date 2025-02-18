using AirBro.TelegramBot.Models;

namespace AirBro.TelegramBot.Services;

public class TempUserDataService
{
    private readonly Dictionary<long, Location> _usersLocations = new();

    public Location GetUserLocation(long chatId)
    {
        var userLocation = new Location();
        if (!_usersLocations.TryAdd(chatId, userLocation))
        {
            userLocation = _usersLocations[chatId];
        }

        return userLocation;
    }
}