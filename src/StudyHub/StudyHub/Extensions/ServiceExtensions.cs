using Microsoft.Extensions.DependencyInjection;
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
}
