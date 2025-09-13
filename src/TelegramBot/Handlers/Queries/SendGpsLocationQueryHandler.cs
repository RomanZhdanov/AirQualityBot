using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SendGpsLocationQueryHandler : IBotQueryHandler
{
    private readonly TempUserDataService  _tempUserDataService;

    public SendGpsLocationQueryHandler(TempUserDataService tempUserDataService)
    {
        _tempUserDataService = tempUserDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        
        var chatId  = callbackQuery.Message!.Chat.Id; 
        var messageId = callbackQuery.Message.MessageId;
        var text = "Send me a location attachment and I'll try to find nearest city to this coordinates.";

        _tempUserDataService.State = UserStates.GpsSearch;
        
        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: text,
            cancellationToken: cancellationToken);    }
}