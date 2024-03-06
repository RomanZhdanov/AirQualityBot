using AirBro.TelegramBot.Handlers;
using AirBro.TelegramBot.Models;
using Microsoft.Extensions.Hosting;

namespace AirBro.TelegramBot;

public class BotService : BackgroundService
{
    private const string BotKey = "6862075580:AAGGDxC9Ut0p0OrweBjl-2FsUCLN-7ZFS30";
    private const string IqAirKey = "499036c0-fec3-4dc1-b256-4452b296b7e1";

    private readonly AirBroBot _bot;
    
    public BotService()
    {
        Dictionary<long, UserProfile> usersData = new();

        IBotHandlers handlers = new BotHandlers(IqAirKey, usersData);
        _bot = new AirBroBot(BotKey, handlers);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        await _bot.StartReceivingAsync(stoppingToken);
    }
}