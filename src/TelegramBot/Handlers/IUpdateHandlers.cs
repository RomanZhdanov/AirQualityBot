using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public interface IUpdateHandlers
{
    Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);

    Task HndleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery query, CancellationToken cancellationToken);

    Task HandleUnknownAsync(ITelegramBotClient botClient, Update update);
}