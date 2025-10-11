namespace AirBro.TelegramBot.Api;

public interface IApiRequest
{
    long ChatId { get; }
    
    int MessageId { get; }
}