using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class CitiesPageQueryHandler : IBotQueryHandler
{
    private readonly IQAirService _airService;

    public CitiesPageQueryHandler(IQAirService airService)
    {
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args =  callbackQuery.Data!.Split('|');
        var country = args[1];
        var state =  args[2];
        var page = Convert.ToInt32(args[3]);
        var citiesPage = await _airService.GetCitiesPage(country, state, page, 10);
        var keyboard = MarkupHelper.GetCitiesPage(country, state, citiesPage);
            
        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: $"State {state} has been saved. Finally select city ({citiesPage.TotalCount} available, page {citiesPage.PageNumber}/{citiesPage.TotalPages})",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}