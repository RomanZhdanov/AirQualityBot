using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Commands.Welcome;

public class WelcomeCommandHandler : IBotCommandHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[] args, CancellationToken cancellationToken)
    {
        var msg = new StringBuilder();
        msg.AppendLine($"Бот представляет из себя дополнение к приложению WireGuard\\.");

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: msg.ToString(),
            parseMode: ParseMode.MarkdownV2,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }
}