using System.Text;
using System.Threading.Channels;
using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

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
                    LocationDto curLocation = null;
                    InlineKeyboardMarkup? keyboard = null;
                    var msgText = new StringBuilder();
                    var startText = string.Empty;

                    if (request.Type == QueuedRequestType.FindLocation)
                    {
                        startText = "Air quality report for location you've searching:";
                    }

                    if (request.Type == QueuedRequestType.AirMonitor)
                    {
                        startText = "Air quality report for your monitoring location(s) (AQI US):";
                    }

                    msgText.AppendLine(startText);
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
                            msgText.AppendLine();
                        }
                        else
                        {
                            curLocation = result.LocationDto;
                            msgText.AppendLine($"{result.LocationDto.ToString()}: {result.Aqi} ({result.Quality})");
                            msgText.AppendLine($"Last update: {result.LastUpdate.ToShortTimeString()}");
                            msgText.AppendLine();
                        }
                    }

                    msgText.AppendLine("You can get more information about Air Quality Index with /aqi_guide command.");

                    if (request.Type == QueuedRequestType.FindLocation && curLocation is not null)
                    {
                        var buttons = new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("Add this location to my monitoring list", $"AddLocation|{curLocation.Country}|{curLocation.State}|{curLocation.City}")
                        };
                        keyboard = new InlineKeyboardMarkup(buttons); 
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
                            replyMarkup:keyboard,
                            cancellationToken: stoppingToken
                        );
                    }
                    else
                    {
                        await _botClient.EditMessageText(
                            chatId: _currentChatId,
                            messageId: _currentMessageId,
                            text: msgText.ToString(),
                            replyMarkup: keyboard,
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