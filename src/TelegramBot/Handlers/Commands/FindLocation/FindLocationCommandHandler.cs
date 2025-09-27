using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Commands.FindLocation;

public class FindLocationCommandHandler : IBotCommandHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        
        var msg = "You have 3 options of how to find locations. choose one that's suites you:";
        
        var buttons = new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("Send GPS location", "SendGpsLocation")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("Search", "SearchCountry")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("Select from list", "CountriesPage|1")
            }
        };
            
        var keyboard = new InlineKeyboardMarkup(buttons);

        await botClient.SendMessage(
            chatId: chatId,
            text: msg,
            replyMarkup: keyboard);
    }
}