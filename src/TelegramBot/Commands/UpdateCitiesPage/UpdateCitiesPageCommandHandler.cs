using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.UpdateCitiesPage;

public class UpdateCitiesPageCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;

    public UpdateCitiesPageCommandHandler(IQAirService airService)
    {
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var country = args[1];
        var state = args[2];
        var page = int.Parse(args[3]);

        var citiesPage = await _airService.GetCitiesPage(country, state, page, 10);
        var keyboard = MarkupHelper.GetCitiesPage(country, state, citiesPage);
            
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: $"State {state} has been saved. Finally select city ({citiesPage.TotalCount} available, page {citiesPage.PageNumber}/{citiesPage.TotalPages})",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}