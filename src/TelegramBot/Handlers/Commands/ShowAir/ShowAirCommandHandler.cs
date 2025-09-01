using System.Text;
using AirBro.TelegramBot.Handlers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Commands.ShowAir;

public class ShowAirCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;
    private readonly UserDataService _usersData;
    
    public ShowAirCommandHandler(IQAirService airService, UserDataService usersData)
    {
        _airService = airService;
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
        
        var msgText = new StringBuilder();
        msgText.AppendLine($"Air quality in your locations (AQI US):");
        msgText.AppendLine();
        
        foreach (var location in locations)
        {
            var result = await _airService.GetAirForCity(location.City, location.State, location.Country.Name);

            msgText.AppendLine($"{result.Location.ToString()}: {result.Aqi} ({result.Quality})");
            msgText.AppendLine($"Last update: {result.LastUpdate.ToShortTimeString()}");
            msgText.AppendLine();
        }
        
        await botClient.SendMessage(
            chatId: chatId,
            text: msgText.ToString(),
            cancellationToken: cancellationToken);
    }
}