using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudyHub.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.ConfigureTelegramBot();
        services.ConfigureAutomapper();
        services.ConfigureServices();
        services.ConfigureSettings(context.Configuration);
    })
    .Build();

host.Run();
