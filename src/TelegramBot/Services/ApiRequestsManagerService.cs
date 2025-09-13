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

    public async Task DispatchGetAirRequestAsync(long chatId, int messageId, IEnumerable<LocationDto> locations)
    {
        var apiRequests = new List<ApiRequest>(); 
        
        foreach (var location in locations) 
        { 
            apiRequests.Add(new ApiRequest(ApiEndpoint.City, location));
        }

        var request = new QueuedRequest(chatId, messageId, apiRequests);
        await _queue.WriteAsync(request);
    }
}