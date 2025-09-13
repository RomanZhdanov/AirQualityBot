using AirBro.TelegramBot.Services;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Commands.AddUserLocation;

public class AddUserLocationCommandHandler : IBotCommandHandler
{
    private readonly int _locationsLimit;
    private readonly UserDataService _userDataService;
    public AddUserLocationCommandHandler(IConfiguration config, UserDataService userDataService)
    {
        _locationsLimit = config.GetValue<int>("LocationsLimit");
        _userDataService = userDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var locationsCount = await _userDataService.GetUserLocationsCountAsync(chatId);

        if (locationsCount >= _locationsLimit)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"You already have maximum of {_locationsLimit} locations.",
                cancellationToken: cancellationToken);

            return;
        }
        
        var msg = $"You can add up to {_locationsLimit} locations. Choose how you would like to add.";
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