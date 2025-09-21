using System.Threading.Channels;
using AirBro.TelegramBot.Models;

namespace AirBro.TelegramBot.Services;

public class ApiRequestsManagerService
{
    private readonly ChannelWriter<QueuedRequest> _queue;

    public ApiRequestsManagerService(ChannelWriter<QueuedRequest> queue)
    {
        _queue = queue;
    }

    public async Task DispatchGetLocationRequestAsync(long chatId, int messageId, LocationDto location)
    {
        var apiRequest = new List<ApiRequest>
        {
            new ApiRequest(ApiEndpoint.City, location)
        };
        
        var request = new QueuedRequest(chatId, messageId, QueuedRequestType.FindLocation, apiRequest);
        await _queue.WriteAsync(request);
    }

    public async Task DispatchFindNearestCityRequestAsync(long chatId, int messageId, double longitude,
        double latitude)
    {
        var location = new LocationDto(longitude, latitude);
        var requests = new List<ApiRequest>
        {
            new ApiRequest(ApiEndpoint.NearestCity, location)
        };
        
        var request = new QueuedRequest(chatId, messageId, QueuedRequestType.FindLocation, requests);
        await _queue.WriteAsync(request);
    }

    public async Task DispatchGetAirRequestAsync(ApiEndpoint endpoint, long chatId, int messageId, IEnumerable<LocationDto> locations)
    {
        var apiRequests = new List<ApiRequest>(); 
        
        foreach (var location in locations) 
        { 
            apiRequests.Add(new ApiRequest(endpoint, location));
        }

        var request = new QueuedRequest(chatId, messageId, QueuedRequestType.AirMonitor, apiRequests);
        await _queue.WriteAsync(request);
    }
}