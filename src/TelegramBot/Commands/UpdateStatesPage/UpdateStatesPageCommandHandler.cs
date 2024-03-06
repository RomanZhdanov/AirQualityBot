using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.UpdateStatesPage;

public class UpdateStatesPageCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;

    public UpdateStatesPageCommandHandler(IQAirService airService)
    {
        _airService = airService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var country = args[1];
        var page = int.Parse(args[2]);
        
        var statesPage = await _airService.GetStatesPage(country, page, 10);
        var keyboard = InlineKeyboardHelper.GetStatesPage(country, statesPage);
            
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: $"Country {country} has been saved. Now select state for that country ({statesPage.TotalCount} available, page {statesPage.PageNumber}/{statesPage.TotalPages})",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}