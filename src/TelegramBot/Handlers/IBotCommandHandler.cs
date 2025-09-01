using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public interface IBotCommandHandler
{
    Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
}