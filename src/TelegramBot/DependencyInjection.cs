using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Extentions;
using AirBro.TelegramBot.Services;
using IQAirApiClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddHostedService<BotService>();
        services.AddSingleton<AirBroBot>();
        services.AddSingleton<IAirQualityService, IQAirService>();
        services.Decorate<IAirQualityService, CachedAirQualityService>();
        services.AddSingleton<TempUserDataService>();
        services.AddScoped<UserDataService>();
        services.AddTransient<IQAirApi, IQAirApiClient.IQAirApiClient>();
        services.AddTransient<LocationsInitializer>();
        services.AddTransient<CountriesService>();
        services.AddHttpClient();
        services.AddBotHandlers();
        services.AddMemoryCache();
        
        return services;
    }
}