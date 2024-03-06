using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AirBro.TelegramBot.Handlers;

public sealed class BotHandlers : IBotHandlers
{
    private readonly IServiceProvider _serviceProvider;

    public BotHandlers(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        
        IUpdateHandlers updateHandlers = scope.ServiceProvider.GetRequiredService<IUpdateHandlers>();
            
        var handler = update.Type switch
        {
            UpdateType.Message => updateHandlers.BotOnMessageReceived(botClient, update.Message, cancellationToken),
            UpdateType.EditedMessage => updateHandlers.BotOnMessageReceived(botClient, update.EditedMessage, cancellationToken),
            UpdateType.CallbackQuery => updateHandlers.BotOnCallbackQueryReceived(botClient, update.CallbackQuery, cancellationToken),
            _ => updateHandlers.UnknownUpdateHandlerAsync(botClient, update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(botClient, exception, cancellationToken);
        }
    }
}