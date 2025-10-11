using System.Threading.Channels;
using AirBro.TelegramBot.Api;
using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Extentions;
using AirBro.TelegramBot.Interfaces;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using IQAirApiClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace AirBro.TelegramBot;

public static class DependencyInjection
{
    public static IServiceCollection AddBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("AirBroBotDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }
        
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var key = configuration["TelegramBotKey"];
            if (key is null) throw new ArgumentException("Bot key is not found!");
            return new TelegramBotClient(key);
        });
        
        var channel = Channel.CreateUnbounded<IApiRequest>();
        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);
        
        var assembly = typeof(Program).Assembly; 
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IApiRequestHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddHostedService<BotService>();
        services.AddHostedService<QueueMonitorService>();
        services.AddSingleton<AirBroBot>();
        services.AddSingleton<IAirApiService, AirVisualApiService>();
        services.Decorate<IAirApiService, CachedAirApiService>();
        services.AddSingleton<TempUserDataService>();
        services.AddScoped<UserDataService>();
        services.AddTransient<IAirVisualApi, IQAirApiClient.AirVisualApiClient>();
        services.AddTransient<LocationsInitializer>();
        services.AddTransient<CountriesService>();
        services.AddTransient<LocationsService>();
        services.AddTransient<ApiRequestsManagerService>();
        services.AddHttpClient();
        services.AddBotHandlers();
        services.AddMemoryCache();
        
        return services;
    }
}