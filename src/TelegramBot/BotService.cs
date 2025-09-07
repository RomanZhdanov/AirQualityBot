using Microsoft.Extensions.Hosting;

namespace AirBro.TelegramBot;

public class BotService : BackgroundService 
{
    private readonly AirBroBot _bot;
    
    public BotService(AirBroBot bot)
    {
        _bot = bot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _bot.StartReceivingAsync(stoppingToken);
    }
}