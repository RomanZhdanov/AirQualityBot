using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SetCountryQueryHandler : IBotQueryHandler
{
    private readonly TempUserDataService _tempUserDataService;
    private readonly IAirApiService _airService;

    public SetCountryQueryHandler(TempUserDataService tempUserDataService, IAirApiService airService)
    {
        _tempUserDataService = tempUserDataService;
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split('|');
        
        var country = args[1];

        var userLocation = _tempUserDataService.GetUserLocation(chatId);
        userLocation.Country = country;
        _tempUserDataService.State = UserStates.None;

        var statesPage = await _airService.GetStatesPage(country, 1, 10);
        await MessagesHelper.SendStatesPageMessage(botClient, country, statesPage, chatId, messageId, cancellationToken);
    }

   
}