using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using StudyHub.Application.Cache;
using StudyHub.Application.Handlers;
using StudyHub.Application.MapperProfiles;
using StudyHub.Application.Services;
using StudyHub.DataContext.Repositories;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Repositories;
using StudyHub.Infrastructure.Services;
using StudyHub.Infrastructure.Settings;
using Telegram.Bot;

namespace StudyHub.Extensions;
public static class ServiceExtensions
{
    public static void ConfigureTelegramBot(this IServiceCollection services)
    {
        var botToken = Environment.GetEnvironmentVariable("TelegramBotToken");
        if (string.IsNullOrEmpty(botToken))
        {
            throw new InvalidOperationException("Telegram bot token is not configured.");
        }

        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(sp => new TelegramBotClient(botToken));
    }

    public static void ConfigureAutomapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ReminderProfile).Assembly);
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandProcessor, CommandProcessor>();
        services.AddScoped<ICommandHandler, AddReminderCommandHandler>();
        services.AddScoped<ICommandHandler, GetRemindersCommandHandler>();
        services.AddScoped<ICommandHandler, AddTutorFeedbackCommandHandler>();
        services.AddScoped<ICommandHandler, GetTutorFeedbackCommandHandler>();
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<IFeedbackRepository, TutorFeedbackRepository>();
    }

    public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.AddOptions<ReminderCommandSettings>()
                .Configure(options => configuration.GetSection(nameof(ReminderCommandSettings)).Bind(options));

        services.AddOptions<CacheSettings>()
            .Configure(options => configuration.GetSection(nameof(CacheSettings)).Bind(options));
    }

    public static void AddRedis(this IServiceCollection services)
    {
        var redisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString");
        var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);

        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddSingleton(provider => provider.GetService<IConnectionMultiplexer>().GetDatabase());
        services.AddScoped<ICacheService, RedisCacheService>();
    }
}
