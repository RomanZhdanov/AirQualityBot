using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class CitiesPageQueryHandler : IBotQueryHandler
{
    private readonly IAirQualityService _airService;

    public CitiesPageQueryHandler(IAirQualityService airService)
    {
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args =  callbackQuery.Data!.Split('|');
        var country = args[1];
        var state =  args[2];
        var page = Convert.ToInt32(args[3]);
        var citiesPage = await _airService.GetCitiesPage(country, state, page, 10);
        
        await MessagesHelper.SendCitiesPageMessage(botClient, citiesPage, country, chatId, messageId, state, cancellationToken);
    }
}