using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using IQAirApiClient;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AirBro.TelegramBot.Handlers;

public sealed class BotHandlers : IBotHandlers
{
    private readonly IQAirService _airService;
    private readonly Dictionary<long, UserProfile> _usersData;

    public BotHandlers(string iqAirKey, Dictionary<long, UserProfile> usersData)
    {
        var httpClient = new HttpClient();
        IQAirApi iqAirClient = new IQAirApiClient.IQAirApiClient(iqAirKey, httpClient);
        _airService = new IQAirService(iqAirClient);
        _usersData = usersData;
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
        IUpdateHandlers updateHandlers = new UpdateHandlers(_airService, _usersData);
        
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