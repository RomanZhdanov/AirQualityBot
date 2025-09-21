using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SetCityQueryHandler : IBotQueryHandler
{
    private readonly TempUserDataService  _tempDataService;
    private readonly ApiRequestsManagerService _apiRequestsManagerService;

    public SetCityQueryHandler(TempUserDataService tempDataService, ApiRequestsManagerService apiRequestsManagerService)
    {
        _tempDataService = tempDataService;
        _apiRequestsManagerService = apiRequestsManagerService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split('|');
        var city = args[1];

        var userLocation = _tempDataService.GetUserLocation(chatId);
        userLocation.City = city;

        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: $"You've selected {userLocation}, fetching data for this location...",
            cancellationToken: cancellationToken);

        var list = new List<LocationDto> { userLocation };

        await _apiRequestsManagerService.DispatchGetAirRequestAsync(ApiEndpoint.City, chatId, messageId, list);
    }
}