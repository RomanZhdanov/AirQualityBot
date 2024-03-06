using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.SetUserState;

public class SetUserStateCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;
    private readonly Dictionary<long, UserProfile> _usersData;
    
    public SetUserStateCommandHandler(IQAirService airService, Dictionary<long, UserProfile> usersData)
    {
        _airService = airService;
        _usersData = usersData;
    }
    
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var state = args[1];
            
        UserProfile userProfile = new UserProfile();
            
        if (!_usersData.TryAdd(chatId, userProfile))
        {
            userProfile = _usersData[chatId];
        }

        userProfile.State = state;

        var citiesPage = await _airService.GetCitiesPage(userProfile.Country, userProfile.State, 1, 10);
        var keyboard = MarkupHelper.GetCitiesPage(userProfile.Country, state, citiesPage);

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: $"State {state} has been saved. Finally select city ({citiesPage.TotalCount} available, page {citiesPage.PageNumber}/{citiesPage.TotalPages})",
            replyMarkup: keyboard,
             cancellationToken: cancellationToken);
    }
}