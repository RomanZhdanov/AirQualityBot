using System.Text;
using IQAirApiClient;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var httpClient = new HttpClient();
IQAirApi iqAirClient = new IQAirApiClient.IQAirApiClient("499036c0-fec3-4dc1-b256-4452b296b7e1", httpClient);
var botClient = new TelegramBotClient("6862075580:AAGGDxC9Ut0p0OrweBjl-2FsUCLN-7ZFS30");

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

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    var result = await iqAirClient.GetSpecifiedCityData("Moscow", "Moscow", "Russia");
    var msgText = new StringBuilder();
    msgText.AppendLine("Pollution in Moscow:");
    msgText.AppendLine($"AQI US: {result.Current.Pollution.Aqius}");
    msgText.AppendLine($"AQI China: {result.Current.Pollution.Aqicn}");
    msgText.AppendLine($"main pollutant for US AQI: {result.Current.Pollution.Mainus}");
    msgText.AppendLine($"main pollutant for Chinese AQI: {result.Current.Pollution.Maincn}");
    
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: msgText.ToString(),
        cancellationToken: cancellationToken);
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