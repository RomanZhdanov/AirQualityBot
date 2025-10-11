using System.Text;
using AirBro.TelegramBot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Api;

public abstract class BaseApiRequestHandler<TRequest> : IApiRequestHandler<TRequest> where TRequest : IApiRequest
{
    private long _currentChatId;
    private int _currentMessageId;
    private bool _needNotification;

    protected readonly StringBuilder ResponseTextBuilder = new();
    protected InlineKeyboardMarkup Keyboard = new();

    private readonly ILogger<BaseApiRequestHandler<TRequest>> _logger;

    protected readonly IAirApiService ApiService;
    protected readonly ITelegramBotClient BotClient;

    protected BaseApiRequestHandler(ILogger<BaseApiRequestHandler<TRequest>> logger, IAirApiService apiService, ITelegramBotClient botClient)
    {
        _logger = logger;
        ApiService = apiService;
        BotClient = botClient;
        
        ApiService.ApiLimitReached += OnApiLimitReached;
    }

    public async Task HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        _currentChatId = request.ChatId;
        _currentMessageId = request.MessageId;

        try
        {
            await HandleRequestAsync(request, cancellationToken);
            
            if (_needNotification)
            {
                await BotClient.DeleteMessage(
                    chatId: _currentChatId,
                    messageId: _currentMessageId,
                    cancellationToken: cancellationToken);

                await BotClient.SendMessage(
                    chatId: _currentChatId,
                    text: ResponseTextBuilder.ToString(),
                    replyMarkup: Keyboard,
                    cancellationToken: cancellationToken 
                );
            }
            else
            {
                await BotClient.EditMessageText(
                    chatId: _currentChatId,
                    messageId: _currentMessageId,
                    text: ResponseTextBuilder.ToString(),
                    replyMarkup: Keyboard,
                    cancellationToken: cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while processing request");
            
            await BotClient.SendMessage(
                chatId: _currentChatId,
                text: $"There was an error: {e.Message}",
                cancellationToken: cancellationToken);
        }
    }
    
    protected abstract Task HandleRequestAsync(TRequest request, CancellationToken cancellationToken);
    
    private async void OnApiLimitReached(object? sender, EventArgs e)
    {
        try
        {
            _needNotification = true;
        
            await BotClient.EditMessageText(
                chatId: _currentChatId,
                messageId: _currentMessageId,
                text: "Your request is taking a little longer to complete because we've reached the AirVisual API limit of requests per minute. We need to wait a bit, so please be patient 🙏");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while sending notification");
        }
    }
}