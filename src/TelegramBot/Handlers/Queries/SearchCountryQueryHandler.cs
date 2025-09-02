using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SearchCountryQueryHandler : IBotQueryHandler
{
    private readonly TempUserDataService _tempUserDataService;

    public SearchCountryQueryHandler(TempUserDataService tempUserDataService)
    {
        _tempUserDataService = tempUserDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId  = callbackQuery.Message!.Chat.Id; 
        var messageId = callbackQuery.Message.MessageId;
        var text = "Just send me a country name and I'll try to find it.";

        _tempUserDataService.State = UserStates.CountrySearch;
        
        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: text,
            cancellationToken: cancellationToken);
    }
}