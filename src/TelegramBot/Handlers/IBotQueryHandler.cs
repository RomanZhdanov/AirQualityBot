using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public interface IBotQueryHandler
{
    Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken);
}