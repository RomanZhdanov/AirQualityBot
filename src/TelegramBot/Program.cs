using System.Reflection;
using AirBro.TelegramBot;
using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Services;
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
        services.AddBotServices(ctx.Configuration);
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
