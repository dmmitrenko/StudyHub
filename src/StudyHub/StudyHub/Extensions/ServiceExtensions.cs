using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddScoped<IReminderRepository, ReminderRepository>();
    }

    public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.AddOptions<ReminderCommandSettings>()
                .Configure(options => configuration.GetSection(nameof(ReminderCommandSettings)).Bind(options));
    }
}
