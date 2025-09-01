using System.Reflection;
using AirBro.TelegramBot;
using AirBro.TelegramBot.Commands;
using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Handlers;
using AirBro.TelegramBot.Services;
using IQAirApiClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(app =>
    {
        app.AddJsonFile("appsettings.json");
        app.AddUserSecrets(Assembly.GetExecutingAssembly());
    })
    .ConfigureServices((ctx, services) =>
    {
        if (ctx.Configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("AirBroBotDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    ctx.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }
        
        services.AddHostedService<BotService>();
        services.AddSingleton<AirBroBot>();
        services.AddSingleton<IQAirService>();
        services.AddSingleton<TempUserDataService>();
        services.AddSingleton<IBotHandlers, BotHandlers>();
        services.AddScoped<IUpdateHandlers, UpdateHandlers>();
        services.AddScoped<UserDataService>();
        services.AddScoped<CommandsManager>();
        services.AddTransient<IQAirApi, IQAirApiClient.IQAirApiClient>();
        services.AddTransient<LocationsInitializer>();
        services.AddHttpClient();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (context.Database.IsSqlite())
        {
            context.Database.Migrate();
        }
        
        var locationsInit = services.GetRequiredService<LocationsInitializer>();
        await locationsInit.StartAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        throw;
    }
}

await host.RunAsync();
