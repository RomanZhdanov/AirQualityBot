using AirBro.TelegramBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace AirBro.TelegramBot;

public sealed class AirBroBot
{
    private readonly ILogger<AirBroBot> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IBotHandlers _handlers;

    public AirBroBot(ILogger<AirBroBot> logger, ITelegramBotClient bot, IBotHandlers handlers)
    {
        _bot = bot;
        _logger = logger;
        _handlers = handlers;
    }
    
    public async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        await _bot.ReceiveAsync(
            updateHandler: _handlers.HandleUpdateAsync,
            errorHandler: _handlers.HandleErrorAsync,
            receiverOptions: new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken: cancellationToken);
        
        var me = await _bot.GetMe(cancellationToken: cancellationToken);

        _logger.LogInformation($"Start listening for @{me.Username}");
    }
}