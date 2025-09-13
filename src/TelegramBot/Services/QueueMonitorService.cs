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
    private long _currentChatId = 0;
    private int _currentMessageId = 0;
    private bool _needNotification = false;

    public QueueMonitorService(ILogger<QueueMonitorService> logger, ChannelReader<QueuedRequest> queue, IAirApiService apiService, ITelegramBotClient botClient)
    {
        _logger = logger;
        _queue = queue;
        _apiService = apiService;
        _apiService.ApiLimitReached += OnApiLimitReached;
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
                    _currentChatId = request.ChatId;
                    _currentMessageId = request.MessageId;
                    _needNotification = false;
                    var msgText = new StringBuilder();
                    msgText.AppendLine($"Air quality in your locations (AQI US):");
                    msgText.AppendLine();
                
                    foreach (var apiRequest in request.ApiRequests)
                    {
                        var location = apiRequest.LocationDto;
                        AirQualityResult? result = null;

                        switch (apiRequest.Endpoint)
                        {
                            case ApiEndpoint.City:
                                result = await _apiService.GetAir(location.Country, location.State, location.City);
                                break;
                            case ApiEndpoint.NearestCity:
                                result = await _apiService.GetNearestCityAir(location.Latitude.Value, location.Longitude.Value);
                                break;
                        }

                        if (result is null)
                        {
                            msgText.AppendLine($"Can't fetch data for location  {location.City}, {location.State}, {location.Country}");
                        }
                        else
                        {
                            msgText.AppendLine($"{result.LocationDto.ToString()}: {result.Aqi} ({result.Quality})");
                            msgText.AppendLine($"Last update: {result.LastUpdate.ToShortTimeString()}");
                            msgText.AppendLine();
                        }
                    }

                    if (_needNotification)
                    {
                        await _botClient.DeleteMessage(
                            chatId: _currentChatId,
                            messageId: _currentMessageId,
                            cancellationToken: stoppingToken);

                        await _botClient.SendMessage(
                            chatId: _currentChatId,
                            text: msgText.ToString(),
                            cancellationToken: stoppingToken
                        );
                    }
                    else
                    {
                        await _botClient.EditMessageText(
                            chatId: _currentChatId,
                            messageId: _currentMessageId,
                            text: msgText.ToString(),
                            cancellationToken: stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occured while processing queue request"); 
                }
                
            }
        }
    }

    private async void OnApiLimitReached(object? sender, EventArgs e)
    {
        _logger.LogWarning("Hello from api limit reached event handler!.");
        _needNotification = true;
        await _botClient.EditMessageText(
            chatId: _currentChatId,
            messageId: _currentMessageId,
            text: "Your request is taking a little longer to complete because we've reached the AirVisual API limit of requests per minute. We need to wait a bit, so please be patient 🙏"); 
    }
}