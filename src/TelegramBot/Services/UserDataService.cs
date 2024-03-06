using AirBro.TelegramBot.Models;

namespace AirBro.TelegramBot.Services;

public class UserDataService
{
    private readonly Dictionary<long, UserProfile> _usersData = new();

    public UserProfile GetUserProfile(long chatId)
    {
        UserProfile userProfile = new UserProfile();
            
        if (!_usersData.TryAdd(chatId, userProfile))
        {
            userProfile = _usersData[chatId];
        }

        return userProfile;
    }
}