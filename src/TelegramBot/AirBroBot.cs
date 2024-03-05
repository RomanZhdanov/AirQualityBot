using AirBro.TelegramBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace AirBro.TelegramBot;

public sealed class AirBroBot
{
    private readonly ITelegramBotClient _bot;
    private readonly IBotHandlers _handlers;

    public AirBroBot(string key, IBotHandlers handlers)
    {
        _bot = new TelegramBotClient(key);
        _handlers = handlers;
    }
    
    public async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        _bot.StartReceiving(
            updateHandler: _handlers.HandleUpdateAsync,
            pollingErrorHandler: _handlers.HandleErrorAsync,
            receiverOptions: new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken: cancellationToken);
        
        var me = await _bot.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();
    }
}