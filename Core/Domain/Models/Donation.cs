using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Donation : BaseEntity<int>
    {
        [Required]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public bool IsAnonymous { get; set; } = false;

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

        public List<EmailNotification> Notifications { get; set; } 

    }
}