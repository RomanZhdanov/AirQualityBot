using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Commands.FindNearestCity;

public class FindNearestCityCommandHandler : IBotCommandHandler
{
    private readonly ApiRequestsManagerService  _apiRequestsManagerService;
    private readonly TempUserDataService  _tempUserDataService;

    public FindNearestCityCommandHandler(ApiRequestsManagerService apiRequestsManagerService, TempUserDataService tempUserDataService)
    {
        _apiRequestsManagerService = apiRequestsManagerService;
        _tempUserDataService = tempUserDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var lat = message.Location.Latitude;
        var lon = message.Location.Longitude;
            
        var msg = await botClient.SendMessage(
            chatId: chatId,
            text: "Searching for the nearest location, please wait...",
            cancellationToken: cancellationToken);

        List<LocationDto> locations = [ new LocationDto(lon, lat) ];

        await _apiRequestsManagerService.DispatchGetAirRequestAsync(ApiEndpoint.NearestCity, chatId, msg.Id,
            locations);

        _tempUserDataService.State = UserStates.None;
    }
}