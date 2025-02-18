using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

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
        
        foreach (var location in locations)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: location.ToString(),
                cancellationToken: cancellationToken);
        }
    }
}