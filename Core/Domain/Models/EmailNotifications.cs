using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class EmailNotification : BaseEntity<int>
    {
        public int DonationId { get; set; }
        public Donation Donation { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; }

        [Required]
        public EmailNotificationType Type { get; set; }

        [EmailAddress, Required]
        public string RecipientEmail { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public bool IsSent { get; set; } = false;

        public DateTime? SentAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}