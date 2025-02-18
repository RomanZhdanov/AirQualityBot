using System.Text;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.ShowAir;

public class ShowAirCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;
    private readonly UserDataService _usersData;
    
    public ShowAirCommandHandler(IQAirService airService, UserDataService usersData)
    {
        _airService = airService;
        _usersData = usersData;
    }
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var locations = await _usersData.GetUserLocationsAsync(chatId);

        if (locations.Count == 0)
        {
            await botClient.SendTextMessageAsync(
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
            var result = await _airService.GetAirForCity(location.City, location.State, location.Country);

            msgText.AppendLine($"{result.Location.ToString()}: {result.Aqi} ({result.Quality})");
            msgText.AppendLine($"Last update: {result.LastUpdate.ToShortTimeString()}");
            msgText.AppendLine();
        }
        
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: msgText.ToString(),
            cancellationToken: cancellationToken);
    }
}