using System.Text;
using Bot.Models;
using IQAirApiClient;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var httpClient = new HttpClient();
IQAirApi iqAirClient = new IQAirApiClient.IQAirApiClient("499036c0-fec3-4dc1-b256-4452b296b7e1", httpClient);
var botClient = new TelegramBotClient("6862075580:AAGGDxC9Ut0p0OrweBjl-2FsUCLN-7ZFS30");
Dictionary<long, UserProfile> usersData = new();

using CancellationTokenSource cts = new ();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new ()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    Message respMsg = null;
    var chatId = message.Chat.Id;
    UserProfile userProfile;

    if (usersData.ContainsKey(chatId))
    {
        userProfile = usersData[chatId];
    }
    else
    {
        userProfile = new UserProfile();
        usersData.Add(chatId, userProfile);
    }

    if (userProfile.Flow.LastQuestionAsked != ConversationFlow.Question.None)
    {
        respMsg = await SetUserLocationDialog(botClient, message, userProfile, cancellationToken);
    }
    else
    {
        var action = message.Text!.Split(' ')[0] switch
        {
            "/set_my_location" => SetUserLocationDialog(botClient, message, userProfile, cancellationToken),
            "/show_air" => SendAirQuality(botClient, message, userProfile, cancellationToken),
            _ => SendStartMessage(botClient, message)
        };

        respMsg = await action;
    }
    
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
        var result = await iqAirClient.GetSpecifiedCityData(userProfile.City, userProfile.State, userProfile.Country);
        
        var msgText = new StringBuilder();
        msgText.AppendLine("Pollution in Moscow:");
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

    async Task<Message> SetUserLocationDialog(ITelegramBotClient bot, Message message, UserProfile userProfile, CancellationToken cancellationToken)
    {
        string msg = string.Empty;
        IReplyMarkup keyboard = new ReplyKeyboardRemove();
        
        switch (userProfile.Flow.LastQuestionAsked)
        {
            case ConversationFlow.Question.None:
                msg = "Lets start from choosing a country. Send my a country name.";
                userProfile.Flow.LastQuestionAsked = ConversationFlow.Question.Country;
                break;
            case ConversationFlow.Question.Country:
                userProfile.Country = message.Text;
                msg = $"Country {message.Text} saved. Now lets choose state.";
                userProfile.Flow.LastQuestionAsked = ConversationFlow.Question.State;
                break;
            case ConversationFlow.Question.State:
                userProfile.State = message.Text;
                msg = $"State {message.Text} saved. Finally lets choose a city.";
                userProfile.Flow.LastQuestionAsked = ConversationFlow.Question.City;
                break;
            case ConversationFlow.Question.City:
                userProfile.City = message.Text;
                msg = $"City {message.Text} saved. You are all set!";
                userProfile.Flow.LastQuestionAsked = ConversationFlow.Question.None;
                break;
        }
        
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

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}