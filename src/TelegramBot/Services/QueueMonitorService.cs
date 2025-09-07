using System.Text;
using System.Threading.Channels;
using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AirBro.TelegramBot.Services;

public class QueueMonitorService : BackgroundService
{
    private readonly ILogger<QueueMonitorService> _logger;
    private readonly ChannelReader<QueuedRequest> _queue;
    private readonly IAirApiService _apiService;
    private readonly ITelegramBotClient _botClient;

    public QueueMonitorService(ILogger<QueueMonitorService> logger, ChannelReader<QueuedRequest> queue, IAirApiService apiService, ITelegramBotClient botClient)
    {
        _logger = logger;
        _queue = queue;
        _apiService = apiService;
        _botClient = botClient;
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
                    var msgText = new StringBuilder();
                    msgText.AppendLine($"Air quality in your locations (AQI US):");
                    msgText.AppendLine();
                
                    foreach (var apiRequest in request.ApiRequests)
                    {
                        var location = apiRequest.Location;
                        
                        var result = await _apiService.GetAir(location.Country, location.State, location.City);

                        if (result is null)
                        {
                            msgText.AppendLine($"Can't fetch data for location  {location.City}, {location.State}, {location.Country}");
                        }
                        else
                        {
                            msgText.AppendLine($"{result.Location.ToString()}: {result.Aqi} ({result.Quality})");
                            msgText.AppendLine($"Last update: {result.LastUpdate.ToShortTimeString()}");
                            msgText.AppendLine();
                        }
                    }
                
                    await _botClient.EditMessageText(
                        chatId: request.ChatId,
                        messageId: request.MessageId,
                        text: msgText.ToString(),
                        cancellationToken: stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occured while processing queue request"); 
                }
                
            }
        }
    }
}