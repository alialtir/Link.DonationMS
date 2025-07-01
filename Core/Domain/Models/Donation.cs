using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Donation : BaseEntity<int>
    {
        [Required, Range(1, 100000)]
        public decimal Amount { get; set; }

        public bool isAnonymous { get; set; } = false;

        [Required]
        public DateTime DonationDate { get; set; }

        [Required]
        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        [MaxLength(300)]
        public string? SessionId { get; set; }

        [MaxLength(300)]
        public string? PaymentIntentId { get; set; }

        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public Guid? UserId { get; set; }
        public User User { get; set; }

        public Receipt Receipt { get; set; } 

        public List<EmailNotifications> Notifications { get; set; } 

    }
}