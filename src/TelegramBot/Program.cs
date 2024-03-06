using AirBro.TelegramBot;
using AirBro.TelegramBot.Handlers;
using AirBro.TelegramBot.Services;
using IQAirApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(app =>
    {
        app.AddJsonFile("appsettings.json");
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<BotService>();
        services.AddSingleton<AirBroBot>();
        services.AddSingleton<IQAirService>();
        services.AddSingleton<IBotHandlers, BotHandlers>();
        services.AddTransient<IQAirApi, IQAirApiClient.IQAirApiClient>();
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();
