using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Domain.Models
{
    /// <summary>
    /// Represents the notification type and its template.
    /// Matches user requirements with no additional properties.
    /// </summary>
    public class NotificationType : BaseEntity<int>
    {
        public NotificationTypeId TypeId { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty; 

        public NotificationLanguage LanguageId { get; set; } 

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
