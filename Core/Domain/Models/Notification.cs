using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Represents a notification sent to a user via any channel (email, SMS, etc.).
    /// Built exactly per user requirements with no additional properties.
    /// </summary>
    public class Notification : BaseEntity<int>
    {
        public string? To { get; set; }
        public string? CC { get; set; }
        public string? BCC { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public NotificationLanguage LanguageId { get; set; } 

        public int NotificationTypeId { get; set; } 
        public NotificationType NotificationType { get; set; }

        //public string? Parameters { get; set; }
        public bool IsSent { get; set; } = false;

        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
