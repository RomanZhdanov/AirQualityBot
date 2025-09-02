using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Commands.AddUserLocation;

public class AddUserLocationCommandHandler : IBotCommandHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        
        var msg = $"Lets start from choosing a country.";
        var buttons = new List<InlineKeyboardButton>();
            
        buttons.Add(InlineKeyboardButton.WithCallbackData("Search", "SearchCountry"));
        buttons.Add(InlineKeyboardButton.WithCallbackData("Select from list", "CountriesPage|1"));
            
        var keyboard = new InlineKeyboardMarkup(buttons);

        await botClient.SendMessage(
            chatId: chatId,
            text: msg,
            replyMarkup: keyboard);
    }
}