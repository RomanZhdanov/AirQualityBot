using AirBro.TelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.SetUserCity;

public class SetUserCityCommandHandler : IBotCommandHandler
{
    private readonly Dictionary<long, UserProfile> _usersData;

    public SetUserCityCommandHandler(Dictionary<long, UserProfile> usersData)
    {
        _usersData = usersData;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var city = args[1];
            
        UserProfile userProfile = new UserProfile();
            
        if (!_usersData.TryAdd(chatId, userProfile))
        {
            userProfile = _usersData[chatId];
        }

        userProfile.City = city;

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: $"You are all set! Use /show_air command to see air quality in your city!",
            cancellationToken: cancellationToken);
    }
}