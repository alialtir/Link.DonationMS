using Domain.Contracts;
using Domain.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimeKit;
using Persistence;
using Persistence.Data;
using Services;
using Services.Abstractions;

namespace Link.DonationMS.NotificationSender
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "My Windows Service";
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<DonationDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddIdentityCore<User>()
                        .AddEntityFrameworkStores<DonationDbContext>();

                    services.AddScoped<IUnitOfWork, UnitOfWork>();

                    services.AddScoped<INotificationSenderService, NotificationSenderService>();

                    services.AddScoped<IEmailSender, MailKitEmailSender>();

                    services.AddHostedService<NotificationWorker>();
                })
                .Build();


            await host.RunAsync();
        }
    }
}
