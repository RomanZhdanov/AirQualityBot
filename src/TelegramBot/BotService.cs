using Microsoft.Extensions.Hosting;

namespace AirBro.TelegramBot;

public class BotService : IHostedService
{
    private const string BotKey = "6862075580:AAGGDxC9Ut0p0OrweBjl-2FsUCLN-7ZFS30";
    private const string IqAirKey = "499036c0-fec3-4dc1-b256-4452b296b7e1";

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