using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public interface IUpdateHandlers
{
    Task BotOnMessageReceived(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);

    Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery query, CancellationToken cancellationToken);

    Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update);
}