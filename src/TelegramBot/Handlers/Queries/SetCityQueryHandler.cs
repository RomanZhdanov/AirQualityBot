using AirBro.TelegramBot.Services;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SetCityQueryHandler : IBotQueryHandler
{
    private readonly int _locationsLimit;
    private readonly TempUserDataService  _tempDataService;
    private readonly UserDataService _usersDataService;

    public SetCityQueryHandler(IConfiguration config, TempUserDataService tempDataService, UserDataService usersDataService)
    {
        _locationsLimit = config.GetValue<int>("LocationsLimit");
        _tempDataService = tempDataService;
        _usersDataService = usersDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split('|');
        var city = args[1];

        var locationsCount = await _usersDataService.GetUserLocationsCountAsync(chatId);

        if (locationsCount >= _locationsLimit)
        {
            await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: "You already reached max locations.",
                cancellationToken: cancellationToken);
            
            return;
        }
        var userLocation = _tempDataService.GetUserLocation(chatId);
        userLocation.City = city;

        await _usersDataService.AddUserLocationAsync(chatId, userLocation);

        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: $"You are all set! Use /show_air command to see air quality in your city!",
            cancellationToken: cancellationToken);
    }
}