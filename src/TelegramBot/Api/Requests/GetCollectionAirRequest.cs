using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AirBro.TelegramBot.Api.Requests;

public record GetCollectionAirRequest(long ChatId, int MessageId, IEnumerable<LocationDto> locations) : IApiRequest;

public class GetCollectionAirRequestHandler : BaseApiRequestHandler<GetCollectionAirRequest>
{
    public GetCollectionAirRequestHandler(ILogger<BaseApiRequestHandler<GetCollectionAirRequest>> logger, IAirApiService apiService, ITelegramBotClient botClient) : base(logger, apiService, botClient)
    {
    }

    protected override async Task HandleRequestAsync(GetCollectionAirRequest request, CancellationToken cancellationToken)
    {
        ResponseTextBuilder.AppendLine("Air quality report for your monitoring location(s) (AQI US):");
        ResponseTextBuilder.AppendLine();

        foreach (var location in request.locations)
        {
            AirQualityResult? result = null;
            
            if (location.Latitude is null || location.Longitude is null)
            {
                result = await ApiService.GetAir(location.Country, location.State, location.City);
            }
            else
            {
                result = await ApiService.GetNearestCityAir(location.Latitude.Value, location.Longitude.Value);
            }        
            
            if (result is null)
            {
                ResponseTextBuilder.AppendLine($"Can't fetch data for location  {location}");
                ResponseTextBuilder.AppendLine();
                continue;           
            }
        
            ResponseTextBuilder.AppendLine($"{result.LocationDto.ToString()}: {result.Aqi} ({result.Quality})");
            ResponseTextBuilder.AppendLine($"Last update: {result.LastUpdate.ToShortTimeString()}");
            ResponseTextBuilder.AppendLine();
        }
        
        ResponseTextBuilder.AppendLine("You can get more information about Air Quality Index with /aqi_guide command.");
    }
}