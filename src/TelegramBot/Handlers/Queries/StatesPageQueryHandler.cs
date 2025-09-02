using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class StatesPageQueryHandler : IBotQueryHandler
{
    private readonly IAirQualityService _airService;

    public StatesPageQueryHandler(IAirQualityService airService)
    {
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args =  callbackQuery.Data!.Split('|');
        var country =  args[1];
        var page = Convert.ToInt32(args[2]);
        var statesPage = await _airService.GetStatesPage(country, page, 10);
        
        await MessagesHelper.SendStatesPageMessage(botClient, country, statesPage, chatId, messageId, cancellationToken);
    }
}