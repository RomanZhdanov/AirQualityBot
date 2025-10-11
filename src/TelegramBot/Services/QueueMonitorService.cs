using System.Threading.Channels;
using AirBro.TelegramBot.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AirBro.TelegramBot.Services;

public class QueueMonitorService : BackgroundService
{
    private readonly ILogger<QueueMonitorService> _logger;
    private readonly ChannelReader<IApiRequest> _queue;
    private readonly IServiceProvider _serviceProvider;

    public QueueMonitorService(ILogger<QueueMonitorService> logger, ChannelReader<IApiRequest> queue, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _queue = queue;
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queue Monitor Service is running.");
        
        while (await _queue.WaitToReadAsync(stoppingToken))
        {
            if (_queue.TryRead(out var request))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    
                    Type requestType = request.GetType();
                    Type handlerType = typeof(IApiRequestHandler<>).MakeGenericType(requestType);
                    var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                    var handleMethod = handler.GetType().GetMethod("HandleAsync");

                    if (handleMethod is not null)
                    {
                        await (Task)handleMethod.Invoke(handler, new object[] { request, stoppingToken });
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occured while processing queue request"); 
                }
            }
        }
    }
}