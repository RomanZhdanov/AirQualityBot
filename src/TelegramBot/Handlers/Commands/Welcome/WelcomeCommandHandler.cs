using System.Text;
using AirBro.TelegramBot.Handlers;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Commands.Welcome;

public class WelcomeCommandHandler : IBotCommandHandler
{
    private readonly int _locationsLimit;

    public WelcomeCommandHandler(IConfiguration config)
    {
        _locationsLimit = config.GetValue<int>("LocationsLimit");
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var msg = new StringBuilder();
        msg.AppendLine($"This bot can help you to monitor air quality in multiple locations. Start by using /find_location to find location and add it to your monitor list. You can add up to {_locationsLimit} locations to your list and you can edit it with /monitor_list command. Use /air_monitor command to check air quality in your locations list. Use /aqi_guide to get more information about Air Quality Index (AQI).");

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: msg.ToString(),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }
}