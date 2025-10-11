using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Api.Requests;

public record GetCityAirByCoordinatesRequest(long ChatId, int MessageId, bool Search, double Latitude, double Longitude) : IApiRequest;

public class GetCityAirByCoordinatesRequestHandler : BaseApiRequestHandler<GetCityAirByCoordinatesRequest>
{
    private readonly LocationsService _locationsService;
    
    public GetCityAirByCoordinatesRequestHandler(ILogger<BaseApiRequestHandler<GetCityAirByCoordinatesRequest>> logger, IAirApiService apiService, ITelegramBotClient botClient, LocationsService locationsService) : base(logger, apiService, botClient)
    {
        _locationsService = locationsService;
    }

    protected override async Task HandleRequestAsync(GetCityAirByCoordinatesRequest request, CancellationToken cancellationToken)
    {
        var result = await ApiService.GetNearestCityAir(request.Latitude, request.Longitude);

        if (result is null)
        {
            ResponseTextBuilder.AppendLine($"Can't fetch data for location  {request.Latitude}, {request.Longitude}");
            return;
        }
        
        if (request.Search) 
            ResponseTextBuilder.AppendLine("Air quality report for location you've searching:");
        
        ResponseTextBuilder.AppendLine($"{result.LocationDto.ToString()}: {result.Aqi} ({result.Quality})");
        ResponseTextBuilder.AppendLine($"Last update: {result.LastUpdate.ToShortTimeString()}");
        ResponseTextBuilder.AppendLine();
        ResponseTextBuilder.AppendLine("You can get more information about Air Quality Index with /aqi_guide command.");

        if (request.Search)
        {
            var locDto = result.LocationDto;
            var location = await _locationsService.GetLocationAsync(locDto.Country, locDto.State, locDto.City);

            if (location is null)
            {
                location = await _locationsService.AddLocationAsync(locDto);
            }

            var queryData = $"AddLocation|{location.Id}";
            Keyboard.AddButton(
                InlineKeyboardButton.WithCallbackData("Add this location to my monitoring list", queryData));
        }
    }
}