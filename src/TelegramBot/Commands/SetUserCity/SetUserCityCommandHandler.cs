using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.SetUserCity;

public class SetUserCityCommandHandler : IBotCommandHandler
{
    private readonly UserDataService _usersData;

    public SetUserCityCommandHandler(UserDataService usersData)
    {
        _usersData = usersData;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var city = args[1];

        await _usersData.SaveUserLocationCityAsync(chatId, city);

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: $"You are all set! Use /show_air command to see air quality in your city!",
            cancellationToken: cancellationToken);
    }
}