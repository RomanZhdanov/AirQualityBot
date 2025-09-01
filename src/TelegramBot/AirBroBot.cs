using AirBro.TelegramBot.Handlers;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace AirBro.TelegramBot;

public sealed class AirBroBot
{
    private readonly ITelegramBotClient _bot;
    private readonly IBotHandlers _handlers;

    public AirBroBot(IConfiguration configuration, IBotHandlers handlers)
    {
        var key = configuration["TelegramBotKey"];
        if (key is null) throw new ArgumentException("Bot key is not found!");
        
        _bot = new TelegramBotClient(key);
        _handlers = handlers;
    }
    
    public async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        _bot.StartReceiving(
            updateHandler: _handlers.HandleUpdateAsync,
            errorHandler: _handlers.HandleErrorAsync,
            receiverOptions: new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken: cancellationToken);
        
        var me = await _bot.GetMe(cancellationToken: cancellationToken);

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();
    }
}