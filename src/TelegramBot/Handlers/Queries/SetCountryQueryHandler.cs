using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SetCountryQueryHandler : IBotQueryHandler
{
    private readonly TempUserDataService _tempUserDataService;

    public SetCountryQueryHandler(TempUserDataService tempUserDataService)
    {
        _tempUserDataService = tempUserDataService;
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

        var msg = $"Country {country} has been saved. Now you need to select state for that country";
        
        var buttons = new List<InlineKeyboardButton>();
            
        buttons.Add(InlineKeyboardButton.WithCallbackData("Select state from list", $"StatesPage|{country}|1"));
            
        var keyboard = new InlineKeyboardMarkup(buttons);
        
        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: msg,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}