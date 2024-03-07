using System.Reflection;
using AirBro.TelegramBot;
using AirBro.TelegramBot.Commands;
using AirBro.TelegramBot.Data;
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
        app.AddUserSecrets(Assembly.GetExecutingAssembly());
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddDbContext<ApplicationDbContext>();
        services.AddHostedService<BotService>();
        services.AddSingleton<AirBroBot>();
        services.AddSingleton<IQAirService>();
        services.AddSingleton<UserDataService>();
        services.AddSingleton<IBotHandlers, BotHandlers>();
        services.AddScoped<IUpdateHandlers, UpdateHandlers>();
        services.AddScoped<CommandsManager>();
        services.AddTransient<IQAirApi, IQAirApiClient.IQAirApiClient>();
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();
