using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SetStateQueryHandler : IBotQueryHandler
{
    private readonly IAirQualityService _airService;
    private readonly TempUserDataService  _userDataService;

    public SetStateQueryHandler(IAirQualityService airService, TempUserDataService userDataService)
    {
        _airService = airService;
        _userDataService = userDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId =  callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split('|');
        var country = args[1];
        var state = args[2];

        var userLocation = _userDataService.GetUserLocation(chatId);
        userLocation.State = state;
        
        
        var citiesPage = await _airService.GetCitiesPage(country, state, 1, 10);

        await MessagesHelper.SendCitiesPageMessage(botClient, citiesPage, country, chatId, messageId, state, cancellationToken);
    }

    
}