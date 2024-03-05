using System.Text;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using IQAirApiClient.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;

namespace AirBro.TelegramBot.Handlers;

public class UpdateHandlers : IUpdateHandlers
{
    private readonly IQAirService _airService;
    private readonly Dictionary<long, UserProfile> _usersData; 

    public UpdateHandlers(IQAirService airService, Dictionary<long, UserProfile> usersData)
    {
        _airService = airService;
        _usersData = usersData;
    }
    
    public async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
        
        Message respMsg = null;
        var chatId = message.Chat.Id;
        UserProfile userProfile;

        if (_usersData.ContainsKey(chatId))
        {
            userProfile = _usersData[chatId];
        }
        else
        {
            userProfile = new UserProfile();
            _usersData.Add(chatId, userProfile);
        }

        var action = message.Text!.Split(' ')[0] switch
        {
            "/set_location" => SetUserLocationDialog(botClient, cancellationToken),
            "/show_air" => SendAirQuality(botClient, message, userProfile, cancellationToken),
            _ => SendStartMessage(botClient, message)
        };

        respMsg = await action;
        
        async Task<Message> SendStartMessage(ITelegramBotClient botClient, Message message)
        {
            var msg = new StringBuilder();
            msg.AppendLine($"Бот представляет из себя дополнение к приложению WireGuard\\.");

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: msg.ToString(),
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: new ReplyKeyboardRemove());
        }

        async Task<Message> SendAirQuality(ITelegramBotClient bot, Message message, UserProfile userProfile,
            CancellationToken cancellationToken)
        {
            var result = await _airService.GetAirForCity(userProfile.City, userProfile.State, userProfile.Country);
            
            var msgText = new StringBuilder();
            msgText.AppendLine($"Pollution in {result.City}:");
            msgText.AppendLine($"AQI US: {result.Current.Pollution.Aqius}");
            msgText.AppendLine($"AQI China: {result.Current.Pollution.Aqicn}");
            msgText.AppendLine($"main pollutant for US AQI: {result.Current.Pollution.Mainus}");
            msgText.AppendLine($"main pollutant for Chinese AQI: {result.Current.Pollution.Maincn}");
            msgText.AppendLine(
                $"Date: {result.Current.Pollution.Ts.ToShortDateString()} Time: {result.Current.Pollution.Ts.ToShortTimeString()}");
            
            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: msgText.ToString(),
                cancellationToken: cancellationToken);
        }

        async Task<Message> SetUserLocationDialog(ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            string msg = string.Empty;
            IReplyMarkup keyboard = new ReplyKeyboardRemove();
            
            var countriesPage = _airService.GetCountriesPage(1, 10);
            msg = $"Lets start from choosing a country. Select country ({countriesPage.TotalCount} available, page {countriesPage.PageNumber}/{countriesPage.TotalPages})";
            keyboard = GetCountriesInlineList(countriesPage);
            
            return await bot.SendTextMessageAsync(
                chatId: chatId,
                text: msg,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }

        // Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        //
        // var result = await iqAirClient.GetSpecifiedCityData("Moscow", "Moscow", "Russia");
        // var msgText = new StringBuilder();
        // msgText.AppendLine("Pollution in Moscow:");
        // msgText.AppendLine($"AQI US: {result.Current.Pollution.Aqius}");
        // msgText.AppendLine($"AQI China: {result.Current.Pollution.Aqicn}");
        // msgText.AppendLine($"main pollutant for US AQI: {result.Current.Pollution.Mainus}");
        // msgText.AppendLine($"main pollutant for Chinese AQI: {result.Current.Pollution.Maincn}");
        //
        // Message sentMessage = await botClient.SendTextMessageAsync(
        //     chatId: chatId,
        //     text: msgText.ToString(),
        //     cancellationToken: cancellationToken);
    }

    private InlineKeyboardMarkup GetCountriesInlineList(PaginatedList<CountryItem> countriesPage)
    {
        var buttonRows = new List<List<InlineKeyboardButton>>();
        foreach (var country in countriesPage.Items)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(country.Country, $"set_country|{country.Country}")
            };
            buttonRows.Add(buttonRow);
        }

        var navigationButtons = new List<InlineKeyboardButton>();

        if (countriesPage.HasPreviousPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData("<<", $"countries_page|{countriesPage.PageNumber - 1}"));
        }

        if (countriesPage.HasNextPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData(">>", $"countries_page|{countriesPage.PageNumber + 1}"));
        }
        
        buttonRows.Add(navigationButtons);
        
        return new InlineKeyboardMarkup(buttonRows);
    }

    public async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery query, CancellationToken cancellationToken)
    {
        if (query.Message is null)
        {
            return;
        }

        long chatId = query.Message.Chat.Id;
        ICommandHandlers handlers = new CommandHandlers();
        string[] args = query.Data.Split('|');

        if (args != null && args.Length > 0)
        {
            var command = args[0];
            UserProfile userProfile = new UserProfile();
            
            if (!_usersData.TryAdd(chatId, userProfile))
            {
                userProfile = _usersData[chatId];
            }

            var action = command switch
            {
                "set_country" => SetUserCountry(botClient, query, args[1]),
                "set_state" => SetUserState(botClient, query, args[1]),
                "set_city" => SetUserCity(botClient, query, args[1]),
                "countries_page" => UpdateCountriesPage(botClient, query, int.Parse(args[1])),
                "states_page" => UpdateStatesPage(botClient, query, args[1], int.Parse(args[2])),
                "cities_page" => UpdateCitiesPage(botClient, query, args[1], args[2], int.Parse(args[3]))
            };

            await action;
        }
        
        async Task<Message> UpdateCountriesPage(ITelegramBotClient botClient, CallbackQuery query, int page)
        {
            var chatId = query.Message!.Chat.Id;
            var messageId = query.Message!.MessageId;
            
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: query.Id);

            var countriesPage = _airService.GetCountriesPage(page, 10);
            var keyboard = GetCountriesInlineList(countriesPage);
            
            return await botClient.EditMessageTextAsync(chatId: chatId,
                messageId: messageId,
                text: $"Lets start from choosing a country. Select country ({countriesPage.TotalCount} available, page {countriesPage.PageNumber}/{countriesPage.TotalPages})",
                replyMarkup: keyboard);
        }
        
        async Task<Message> UpdateStatesPage(ITelegramBotClient botClient, CallbackQuery query, string country, int page)
        {
            var chatId = query.Message!.Chat.Id;
            var messageId = query.Message!.MessageId;
            
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: query.Id);

            var statesPage = await _airService.GetStatesPage(country, page, 10);
            var keyboard = GetStatesInlineList(country, statesPage);
            
            return await botClient.EditMessageTextAsync(chatId: chatId,
                messageId: messageId,
                text: $"Country {country} has been saved. Now select state for that country ({statesPage.TotalCount} available, page {statesPage.PageNumber}/{statesPage.TotalPages})",
                replyMarkup: keyboard);
        }
        
        async Task<Message> UpdateCitiesPage(ITelegramBotClient botClient, CallbackQuery query, string country, string state, int page)
        {
            var chatId = query.Message!.Chat.Id;
            var messageId = query.Message!.MessageId;
            
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: query.Id);

            var citiesPage = await _airService.GetCitiesPage(country, state, page, 10);
            var keyboard = GetCitiesInlineList(country, state, citiesPage);
            
            return await botClient.EditMessageTextAsync(chatId: chatId,
                messageId: messageId,
                text: $"State {state} has been saved. Finally select city ({citiesPage.TotalCount} available, page {citiesPage.PageNumber}/{citiesPage.TotalPages})",
                replyMarkup: keyboard);
        }

        async Task<Message> SetUserCountry(ITelegramBotClient botClient, CallbackQuery query, string country)
        {
            var chatId = query.Message.Chat.Id;
            var messageId = query.Message.MessageId;
            
            UserProfile userProfile = new UserProfile();
            
            if (!_usersData.TryAdd(chatId, userProfile))
            {
                userProfile = _usersData[chatId];
            }

            userProfile.Country = country;

            var statesPage = await _airService.GetStatesPage(country, 1, 10);
            var keyboard = GetStatesInlineList(country, statesPage);

            return await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId,
                text: $"Country {country} has been saved. Now select state for that country ({statesPage.TotalCount} available, page {statesPage.PageNumber}/{statesPage.TotalPages})",
                replyMarkup: keyboard);
        }
    }

    async Task<Message> SetUserState(ITelegramBotClient botClient, CallbackQuery query, string state)
    {
        var chatId = query.Message.Chat.Id;
        var messageId = query.Message.MessageId;
            
        UserProfile userProfile = new UserProfile();
            
        if (!_usersData.TryAdd(chatId, userProfile))
        {
            userProfile = _usersData[chatId];
        }

        userProfile.State = state;

        var citiesPage = await _airService.GetCitiesPage(userProfile.Country, userProfile.State, 1, 10);
        var keyboard = GetCitiesInlineList(userProfile.Country, state, citiesPage);

        return await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: messageId,
            text: $"State {state} has been saved. Finally select city ({citiesPage.TotalCount} available, page {citiesPage.PageNumber}/{citiesPage.TotalPages})",
            replyMarkup: keyboard);
    }
    
    async Task<Message> SetUserCity(ITelegramBotClient botClient, CallbackQuery query, string city)
    {
        var chatId = query.Message.Chat.Id;
        var messageId = query.Message.MessageId;
            
        UserProfile userProfile = new UserProfile();
            
        if (!_usersData.TryAdd(chatId, userProfile))
        {
            userProfile = _usersData[chatId];
        }

        userProfile.City = city;

        return await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: messageId,
            text: $"You are all set! Use /show_air command to see air quality in your city!");
    }
    
    private InlineKeyboardMarkup GetStatesInlineList(string country, PaginatedList<StateItem> statesPage)
    {
        var buttonRows = new List<List<InlineKeyboardButton>>();
        foreach (var state in statesPage.Items)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(state.State, $"set_state|{state.State}")
            };
            buttonRows.Add(buttonRow);
        }

        var navigationButtons = new List<InlineKeyboardButton>();

        if (statesPage.HasPreviousPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData("<<", $"states_page|{country}|{statesPage.PageNumber - 1}"));
        }

        if (statesPage.HasNextPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData(">>", $"states_page|{country}|{statesPage.PageNumber + 1}"));
        }
        
        buttonRows.Add(navigationButtons);
        
        return new InlineKeyboardMarkup(buttonRows);
    }
    
    private InlineKeyboardMarkup GetCitiesInlineList(string country, string state, PaginatedList<CityItem> citiesPage)
    {
        var buttonRows = new List<List<InlineKeyboardButton>>();
        foreach (var city in citiesPage.Items)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(city.City, $"set_city|{city.City}")
            };
            buttonRows.Add(buttonRow);
        }

        var navigationButtons = new List<InlineKeyboardButton>();

        if (citiesPage.HasPreviousPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData("<<", $"cities_page|{country}|{state}|{citiesPage.PageNumber - 1}"));
        }

        if (citiesPage.HasNextPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData(">>", $"cities_page|{country}|{state}|{citiesPage.PageNumber + 1}"));
        }
        
        buttonRows.Add(navigationButtons);
        
        return new InlineKeyboardMarkup(buttonRows);
    }

    public Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}