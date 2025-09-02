using System.Reflection;
using AirBro.TelegramBot.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace AirBro.TelegramBot.Extentions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddBotHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IBotHandlers, BotHandlers>();
        services.AddSingleton<IUpdateHandlers, UpdateHandlers>();
        services.AddScoped<MessageTextHandler>();
        
        var assembly = Assembly.GetExecutingAssembly();
        var implementations = assembly.GetTypes()
            .Where(type => 
                (typeof(IBotCommandHandler).IsAssignableFrom(type) || typeof(IBotQueryHandler).IsAssignableFrom(type)) && 
                type is { IsInterface: false, IsAbstract: false });

        foreach (var implementation in implementations)
        {
            services.AddScoped(implementation);
        }
        
        return services;
    }
}