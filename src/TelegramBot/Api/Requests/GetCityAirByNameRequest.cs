using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Api.Requests;

public record GetCityAirByNameRequest(long ChatId, int MessageId, bool Search, string Country, string State, string City) : IApiRequest;

public class GetCityAirByNameRequestHandler : BaseApiRequestHandler<GetCityAirByNameRequest>
{
    private readonly LocationsService _locationsService;
    public GetCityAirByNameRequestHandler(
        ILogger<BaseApiRequestHandler<GetCityAirByNameRequest>> logger, 
        IAirApiService apiService, 
        ITelegramBotClient botClient, LocationsService locationsService) 
        : base(logger, apiService, botClient)
    {
        _locationsService = locationsService;
    }

    protected override async Task HandleRequestAsync(GetCityAirByNameRequest request, CancellationToken cancellationToken)
    {
        var result = await ApiService.GetAir(request.Country, request.State, request.City);

        if (result is null)
        {
            ResponseTextBuilder.AppendLine($"Can't fetch data for location  {request.City}, {request.State}, {request.Country}");
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
            var location = await _locationsService.GetLocationAsync(request.Country, request.State, request.City);

            if (location is null)
            {
                location = await _locationsService.AddLocationAsync(result.LocationDto);
            }
            
            var queryData = $"AddLocation|{location.Id}";
            Keyboard.AddButton(InlineKeyboardButton.WithCallbackData("Add this location to my monitoring list", queryData));  
        }
    }
}