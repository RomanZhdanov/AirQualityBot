namespace AirBro.TelegramBot.Models;

public record QueuedRequest(long ChatId, int MessageId, IEnumerable<ApiRequest> ApiRequests)
{
    public Guid Id { get; set; } = Guid.NewGuid();
}