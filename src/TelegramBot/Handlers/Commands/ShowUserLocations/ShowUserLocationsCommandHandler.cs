using AirBro.TelegramBot.Services;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Commands.ShowUserLocations;

public class ShowUserLocationsCommandHandler : IBotCommandHandler
{
    private readonly UserDataService _userData;
    private readonly int _locationsLimit;

    public ShowUserLocationsCommandHandler(IConfiguration config, UserDataService userData)
    {
        _locationsLimit = config.GetValue<int>("LocationsLimit");
        _userData = userData;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        
        var locations = await _userData.GetUserLocationsAsync(chatId);

        if (locations.Count == 0)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"You haven't saved any locations yet! Use the /set_location command, you can add up to {_locationsLimit} locations.",
                cancellationToken: cancellationToken);

            return;
        }
        
        ReplyMarkup keyboard = new ReplyKeyboardRemove();
        var buttonRows = new List<List<InlineKeyboardButton>>();
        
        foreach (var location in locations)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(location.ToString(), $"GetLocationActions|{location.Id}")
            };
            
            buttonRows.Add(buttonRow);
        }
        
        keyboard = new InlineKeyboardMarkup(buttonRows);
            
        await botClient.SendMessage(
            chatId: chatId,
            text: $"Your locations ({locations.Count}/{_locationsLimit}):",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}