using AirBro.TelegramBot.Models;

namespace AirBro.TelegramBot.Services;

public class TempUserDataService
{
    private readonly Dictionary<long, LocationDto> _usersLocations = new();

    public UserStates State { get; set; }

    public LocationDto GetUserLocation(long chatId)
    {
        var userLocation = new LocationDto();
        if (!_usersLocations.TryAdd(chatId, userLocation))
        {
            userLocation = _usersLocations[chatId];
        }

        return userLocation;
    }
}