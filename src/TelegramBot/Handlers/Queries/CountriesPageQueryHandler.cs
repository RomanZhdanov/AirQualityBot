using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class CountriesPageQueryHandler : IBotQueryHandler
{
    private readonly CountriesService _countriesService;

    public CountriesPageQueryHandler(CountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split('|');
        var page = Convert.ToInt32(args[1]);
        var pageSize = 10;
        var countriesPage = await _countriesService.GetCountriesPage(page, pageSize, cancellationToken);
        var msg =
            $"Lets start from choosing a country. Select country ({countriesPage.TotalPages} available, page {countriesPage.PageNumber}/{countriesPage.TotalPages})";
        var keyboard = MarkupHelper.GetCountriesPage(countriesPage);

        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: msg,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}