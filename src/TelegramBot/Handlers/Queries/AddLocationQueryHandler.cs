using AirBro.TelegramBot.Exceptions;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class AddLocationQueryHandler : IBotQueryHandler
{
    private readonly int _locationsLimit;
    private readonly UserDataService _userDataService;

    public AddLocationQueryHandler(IConfiguration config, UserDataService userDataService)
    {
        _locationsLimit = config.GetValue<int>("LocationsLimit");
        _userDataService = userDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    { 
        var chatId = callbackQuery.Message!.Chat.Id;
        var args = callbackQuery.Data!.Split('|');
        var locationId = Convert.ToInt32(args[1]);
            
        var locationsCount = await _userDataService.GetUserLocationsCountAsync(chatId);

        if (locationsCount >= _locationsLimit)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"You already reached max of {_locationsLimit} locations.",
                cancellationToken: cancellationToken);
            
            return;
        }

        try
        {
            var location = await _userDataService.AddUserLocationAsync(chatId, locationId);
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Location {location} was successfully added. Use /air_monitor to watch air in your locations or /monitor_list to manage your location.",
                cancellationToken: cancellationToken);
        }
        catch (LocationNotFoundException)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Location was not found.",
                cancellationToken: cancellationToken);
        }
        catch (LocationAlreadyAddedException)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Location already added.",
                cancellationToken: cancellationToken);
        }
    }
}