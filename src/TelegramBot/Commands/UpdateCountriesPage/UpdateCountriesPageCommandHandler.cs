using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.UpdateCountriesPage;

public class UpdateCountriesPageCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;

    public UpdateCountriesPageCommandHandler(IQAirService airService)
    {
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var page = int.Parse(args[1]);

        var countriesPage = _airService.GetCountriesPage(page, 10);
        var keyboard = MarkupHelper.GetCountriesPage(countriesPage);
            
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: $"Lets start from choosing a country. Select country ({countriesPage.TotalCount} available, page {countriesPage.PageNumber}/{countriesPage.TotalPages})",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}