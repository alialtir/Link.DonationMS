using Domain.Models;

namespace Application.Services.Abstractions
{
    public interface INotificationService
    {
        /// <summary>
        /// Create notification for any type (Generic)
        /// </summary>
        /// <param name="typeId">Notification type enum</param>
        /// <param name="to">Email or recipient</param>
        /// <param name="parameters">Placeholders to be replaced in the template</param>
        /// <param name="language">Language for the notification template (defaults to Arabic)</param>
        Task CreateNotificationAsync(NotificationTypeId typeId, string to, object parameters, NotificationLanguage language = NotificationLanguage.English);
    }
}
