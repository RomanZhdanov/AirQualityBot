using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Commands.SetUserLocation;

public class SetUserLocationCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;

    public SetUserLocationCommandHandler(IQAirService airService)
    {
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        string msg = string.Empty;
        IReplyMarkup keyboard = new ReplyKeyboardRemove();
            
        var countriesPage = _airService.GetCountriesPage(1, 10);
        msg = $"Lets start from choosing a country. Select country ({countriesPage.TotalCount} available, page {countriesPage.PageNumber}/{countriesPage.TotalPages})";
        keyboard = MarkupHelper.GetCountriesPage(countriesPage);
            
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: msg,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}