using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Commands.AddUserLocation;

public class AddUserLocationCommandHandler : IBotCommandHandler
{
    private readonly IQAirService _airService;
    private readonly UserDataService _usersData;
    private readonly TempUserDataService _tempUserData;

    public AddUserLocationCommandHandler(IQAirService airService, UserDataService usersData, TempUserDataService tempUserData)
    {
        _airService = airService;
        _usersData = usersData;
        _tempUserData = tempUserData;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, string[]? args, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var command = args is null
            ? string.Empty
            : args[0];
        
        var action = command switch
        {
            "set_country" => SetCountry(),
            "set_state" => SetState(),
            "set_city" => SetCity(),
            "countries_page" => GetCountriesPage(int.Parse(args[1])),
            "states_page" => GetStatesPage(args[1], int.Parse(args[2])),
            "cities_page" => GetCitiesPage(args[1], args[2], int.Parse(args[3])),
            _ => AddLocationStart()
        };
        await action;
        

        async Task<Message> AddLocationStart()
        {
            return await GetCountriesPage(1, false);
        }

        async Task<Message> GetCountriesPage(int page, bool update = true)
        {
            var countriesPage = _airService.GetCountriesPage(page, 10);
            var msg = $"Lets start from choosing a country. Select country ({countriesPage.TotalCount} available, page {countriesPage.PageNumber}/{countriesPage.TotalPages})";
            var keyboard = MarkupHelper.GetCountriesPage(countriesPage);

            if (update)
            {
                return await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: message.MessageId,
                    text: msg,
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken);
            }
            
            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: msg,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }

        async Task<Message> GetStatesPage(string country, int page)
        {
            var statesPage = await _airService.GetStatesPage(country, page, 10);
            var keyboard = MarkupHelper.GetStatesPage(country, statesPage);
            
            return await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: message.MessageId,
                text: $"Country {country} has been saved. Now select state for that country ({statesPage.TotalCount} available, page {statesPage.PageNumber}/{statesPage.TotalPages})",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }

        async Task<Message> GetCitiesPage(string country, string state, int page)
        {
            var citiesPage = await _airService.GetCitiesPage(country, state, page, 10);
            var keyboard = MarkupHelper.GetCitiesPage(country, state, citiesPage);
            
            return await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: message.MessageId,
                text: $"State {state} has been saved. Finally select city ({citiesPage.TotalCount} available, page {citiesPage.PageNumber}/{citiesPage.TotalPages})",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }
        
        async Task<Message> SetCountry()
        {
            var country = args[1];

            var userLocation = _tempUserData.GetUserLocation(chatId);
            userLocation.Country = country;

            return await GetStatesPage(country, 1);
        }

        async Task<Message> SetState()
        {
            var state = args[1];

            var userLocation = _tempUserData.GetUserLocation(chatId);
            userLocation.State = state;

            return await GetCitiesPage(userLocation.Country, userLocation.State, 1);
        }

        async Task<Message> SetCity()
        {
            var city = args[1];

            var userLocation = _tempUserData.GetUserLocation(chatId);
            userLocation.City = city;

            await _usersData.AddUserLocationAsync(chatId, userLocation);

            return await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: message.MessageId,
                text: $"You are all set! Use /show_air command to see air quality in your city!",
                cancellationToken: cancellationToken);
        }
    }
}