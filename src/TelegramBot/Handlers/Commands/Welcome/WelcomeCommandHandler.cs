using System.Text;
using AirBro.TelegramBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Commands.Welcome;

public class WelcomeCommandHandler : IBotCommandHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var msg = new StringBuilder();
        msg.AppendLine($"Бот представляет из себя дополнение к приложению WireGuard\\.");

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: msg.ToString(),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }
}