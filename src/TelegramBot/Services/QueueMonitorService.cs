using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AirBro.TelegramBot.Services;

public class QueueMonitorService : BackgroundService
{
    private readonly  ILogger<QueueMonitorService> _logger;

    public QueueMonitorService(ILogger<QueueMonitorService> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}