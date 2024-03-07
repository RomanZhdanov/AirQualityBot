using Microsoft.Extensions.Hosting;

namespace AirBro.TelegramBot;

public class BotService : IHostedService
{
    private readonly AirBroBot _bot;
    
    public BotService(AirBroBot bot)
    {
        _bot = bot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _bot.StartReceivingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}