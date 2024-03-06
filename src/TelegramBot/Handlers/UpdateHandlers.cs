using AirBro.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace AirBro.TelegramBot.Handlers;

public class UpdateHandlers : IUpdateHandlers
{
    private readonly CommandsManager _commandsManager;

    public UpdateHandlers(CommandsManager commandsManager)
    {
        _commandsManager = commandsManager;
    }
    
    public async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        await botClient.SendChatActionAsync(
            chatId: message.Chat.Id, 
            chatAction: ChatAction.Typing, 
            cancellationToken: cancellationToken);

        var command = message.Text!.Split(' ')[0];
        var handler = _commandsManager.GetCommandHandler(command);
        await handler.HandleAsync(botClient, message, null, cancellationToken);
    }

    public async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery query, CancellationToken cancellationToken)
    {
        if (query.Message is null || query.Data is null)
        {
            return;
        }

        var args = query.Data.Split('|');

        if (args.Length > 0)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: query.Id, 
                cancellationToken: cancellationToken);

            var command = args[0];
            var handler = _commandsManager.GetCommandHandler(command);
            await handler.HandleAsync(botClient, query.Message, args, cancellationToken);
        }
    }

    public Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}