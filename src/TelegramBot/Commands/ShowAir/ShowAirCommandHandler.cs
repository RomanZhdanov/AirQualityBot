using System.Text;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.ShowAir;

public class ShowAirCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;
    private readonly Dictionary<long, UserProfile> _usersData;
    
    public ShowAirCommandHandler(IQAirService airService, Dictionary<long, UserProfile> usersData)
    {
        _airService = airService;
        _usersData = usersData;
    }
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        
        UserProfile userProfile = new UserProfile();
            
        if (!_usersData.TryAdd(chatId, userProfile))
        {
            userProfile = _usersData[chatId];
        }
        
        var result = await _airService.GetAirForCity(userProfile.City, userProfile.State, userProfile.Country);
            
        var msgText = new StringBuilder();
        msgText.AppendLine($"Ait quality in {result.Location.ToString()}:");
        msgText.AppendLine($"AQI US: {result.Aqi} ({result.Quality})");
        msgText.AppendLine($"LastUpdate: {result.LastUpdate.ToShortTimeString()}");
            
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: msgText.ToString(),
            cancellationToken: cancellationToken);
    }
}