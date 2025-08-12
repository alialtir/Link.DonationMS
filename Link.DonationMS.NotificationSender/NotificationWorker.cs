using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Services.Abstractions;

public class NotificationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationWorker> _logger;

    public NotificationWorker(IServiceProvider serviceProvider, ILogger<NotificationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
        
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationWorker started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var senderService = scope.ServiceProvider.GetRequiredService<INotificationSenderService>();

                _logger.LogInformation("Starting to process pending notifications at: {time}", DateTimeOffset.Now);

                await senderService.ProcessPendingNotificationsAsync();

                _logger.LogInformation("Finished processing notifications at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing notifications.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("NotificationWorker stopping at: {time}", DateTimeOffset.Now);
    }
}
