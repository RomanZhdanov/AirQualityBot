using System.Threading.Channels;
using AirBro.TelegramBot.Api;
using AirBro.TelegramBot.Api.Requests;
using AirBro.TelegramBot.Models;

namespace AirBro.TelegramBot.Services;

public class ApiRequestsManagerService
{
    private readonly ChannelWriter<IApiRequest> _queue;

    public ApiRequestsManagerService(ChannelWriter<IApiRequest> queue)
    {
        _queue = queue;
    }

    public async Task DispatchGetLocationRequestAsync(long chatId, int messageId, bool search, LocationDto location)
    {
        IApiRequest request;
        
        if (location.Latitude is null || location.Longitude is null)
        {
            request =
                new GetCityAirByNameRequest(chatId, messageId, search, location.Country, location.State, location.City);
        }
        else
        {
            request =
                new GetCityAirByCoordinatesRequest(chatId, messageId, search, location.Latitude.Value, location.Longitude.Value);
        }

        await _queue.WriteAsync(request);
    }

    public async Task DispatchFindNearestCityRequestAsync(long chatId, int messageId, bool search, double longitude,
        double latitude)
    {
        var request = new GetCityAirByCoordinatesRequest(chatId, messageId, search, latitude, longitude);
        await _queue.WriteAsync(request);
    }

    public async Task DispatchGetCollectionAirRequestAsync(long chatId, int messageId, IEnumerable<LocationDto> locations)
    {
        var request = new GetCollectionAirRequest(chatId, messageId, locations);
        await _queue.WriteAsync(request);
    }

    public async Task DispatchGetStatesPageRequestAsync(long chatId, int messageId, string country, int page)
    {
        var request = new GetStatesPageRequest(chatId, messageId, country, page);
        await _queue.WriteAsync(request);
    }

    public async Task DispatchGetCitiesPageRequestAsync(long chatId, int messageId, string country, string state,
        int page)
    {
        var request = new GetCitiesPageRequest(chatId, messageId, country, state, page);
        await _queue.WriteAsync(request);   
    }
}