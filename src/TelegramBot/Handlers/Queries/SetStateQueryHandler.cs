using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Queries;

public class SetStateQueryHandler : IBotQueryHandler
{
    private readonly TempUserDataService  _userDataService;

    public SetStateQueryHandler(TempUserDataService userDataService)
    {
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
        
        var msg = $"State {state} has been saved. Now you need to select city for that state";
        
        var buttons = new List<InlineKeyboardButton>();
            
        buttons.Add(InlineKeyboardButton.WithCallbackData("Select city from list", $"CitiesPage|{country}|{state}|1"));
            
        var keyboard = new InlineKeyboardMarkup(buttons);
        
        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: msg,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}