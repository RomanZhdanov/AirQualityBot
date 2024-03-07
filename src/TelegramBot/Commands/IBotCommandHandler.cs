using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands;

public interface IBotCommandHandler
{
    Task HandleAsync(ITelegramBotClient botClient, Message message, string[]? args, CancellationToken cancellationToken);
}