using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Commands.ShowUserLocations;

public class ShowUserLocationsCommandHandler : IBotCommandHandler
{
    private readonly UserDataService _userData;

    public ShowUserLocationsCommandHandler(UserDataService userData)
    {
        _userData = userData;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[]? args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        
        var locations = await _userData.GetUserLocationsAsync(chatId);

        if (locations.Count == 0)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "You haven't set any location yet! Use the /set_location command.",
                cancellationToken: cancellationToken);

            return;
        }
        
        IReplyMarkup keyboard = new ReplyKeyboardRemove();
        var buttonRows = new List<List<InlineKeyboardButton>>();
        
        foreach (var location in locations)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(location.ToString(), $"client_actions|{location.Id}")
            };
            
            buttonRows.Add(buttonRow);
        }
        
        keyboard = new InlineKeyboardMarkup(buttonRows);
            
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Your locations:",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}