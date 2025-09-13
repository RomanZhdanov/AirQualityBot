using AirBro.TelegramBot.Models.Mappers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Commands.ShowAir;

public class ShowAirCommandHandler : IBotCommandHandler
{
    private readonly ApiRequestsManagerService _apiService;
    private readonly UserDataService _usersData;
    
    public ShowAirCommandHandler(ApiRequestsManagerService apiService, UserDataService usersData)
    {
        _apiService = apiService;
        _usersData = usersData;
    }
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var locations = await _usersData.GetUserLocationsAsync(chatId);

        if (locations.Count == 0)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "You haven't set any location yet! Use the /set_location command.",
                cancellationToken: cancellationToken);

            return;
        }
        
        var msg = await botClient.SendMessage(
            chatId: chatId,
            text: "Fetching data please wait...",
            cancellationToken: cancellationToken);

        var loc = locations.Select(l => l.ToLocationDto());
        await _apiService.DispatchGetAirRequestAsync(chatId, msg.Id, loc);
    }
}