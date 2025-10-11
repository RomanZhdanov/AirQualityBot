namespace AirBro.TelegramBot.Api;

public interface IApiRequestHandler<in TRequest> where TRequest : IApiRequest
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}