using Domain.Contracts;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Services.Abstractions;
using Services.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Services
{
    public class NotificationSenderService : INotificationSenderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public NotificationSenderService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task ProcessPendingNotificationsAsync()
        {
            var spec = new PendingNotificationsSpec();
            var pending = await _unitOfWork.Notifications.ListAsync(spec);

            foreach (var notification in pending)
            {
                try
                {
                    await _emailSender.SendEmailAsync(notification.To, notification.Subject, notification.Body);
                    notification.IsSent = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending notification {notification.Id}: {ex.Message}");
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }


}
