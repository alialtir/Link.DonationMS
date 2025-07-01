using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class EmailNotifications : BaseEntity<int>
    {
        public int DonationId { get; set; }
        public Donation Donation { get; set; }

        [Required, MaxLength(100)]
        public string Type { get; set; } 
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

    }
    
    
}