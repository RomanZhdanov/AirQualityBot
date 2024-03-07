using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.SetUserCountry;

public class SetUserCountryCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;
    private readonly UserDataService _usersData;

    public SetUserCountryCommandHandler(IQAirService airService, UserDataService usersData)
    {
        _airService = airService;
        _usersData = usersData;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var country = args[1];

        await _usersData.SaveUserLocationCountryAsync(chatId, country);

        var statesPage = await _airService.GetStatesPage(country, 1, 10);
        var keyboard = MarkupHelper.GetStatesPage(country, statesPage);

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: $"Country {country} has been saved. Now select state for that country ({statesPage.TotalCount} available, page {statesPage.PageNumber}/{statesPage.TotalPages})",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}