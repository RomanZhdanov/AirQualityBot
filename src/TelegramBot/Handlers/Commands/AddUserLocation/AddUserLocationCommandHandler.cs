using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Commands.AddUserLocation;

public class AddUserLocationCommandHandler : IBotCommandHandler
{
    private const int LocationsLimit = 5;
    private readonly UserDataService _userDataService;
    public AddUserLocationCommandHandler(UserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var locationsCount = await _userDataService.GetUserLocationsCountAsync(chatId);

        if (locationsCount >= LocationsLimit)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "You already have maximum locations.",
                cancellationToken: cancellationToken);

            return;
        }
        
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