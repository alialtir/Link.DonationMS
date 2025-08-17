using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Services.Abstractions;

public class NotificationWorker : BackgroundService
{
    private readonly INotificationSenderService _notificationSenderService;
    private readonly ILogger<NotificationWorker> _logger;

    public NotificationWorker(INotificationSenderService notificationSenderService, ILogger<NotificationWorker> logger)
    {
        _notificationSenderService = notificationSenderService;
        _logger = logger;
    }
        
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationWorker started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting to process pending notifications at: {time}", DateTimeOffset.Now);

                await _notificationSenderService.ProcessPendingNotificationsAsync();

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
