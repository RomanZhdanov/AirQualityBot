using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Models.Mappers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;

namespace AirBro.TelegramBot.Handlers.Commands.ShowAir;

public class ShowAirCommandHandler : IBotCommandHandler
{
    private readonly ApiRequestsManagerService _apiService;
    private readonly UserDataService _usersData;
    private readonly int _locationsLimit;

    public ShowAirCommandHandler(IConfiguration config, ApiRequestsManagerService apiService, UserDataService usersData)
    {
        _locationsLimit = config.GetValue<int>("LocationsLimit");
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
                text: $"You haven't added any location yet! Use the /find_location command to find location and then add it to your monitor list, you can add up to {_locationsLimit} locations.",
                cancellationToken: cancellationToken);

            return;
        }

        var msg = await botClient.SendMessage(
            chatId: chatId,
            text: "Fetching data please wait...",
            cancellationToken: cancellationToken);

        var loc = locations.Select(l => l.ToLocationDto());
        await _apiService.DispatchGetAirRequestAsync(ApiEndpoint.City, chatId, msg.Id, loc);
    }
}
