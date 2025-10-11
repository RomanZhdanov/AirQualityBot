using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class StatesPageQueryHandler : IBotQueryHandler
{
    private readonly ApiRequestsManagerService _apiRequestsManagerService;
    
    public StatesPageQueryHandler(ApiRequestsManagerService apiRequestsManagerService)
    {
        _apiRequestsManagerService = apiRequestsManagerService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args =  callbackQuery.Data!.Split('|');
        var country =  args[1];
        var page = Convert.ToInt32(args[2]);

        await _apiRequestsManagerService.DispatchGetStatesPageRequestAsync(chatId, messageId, country, page);
    }
}