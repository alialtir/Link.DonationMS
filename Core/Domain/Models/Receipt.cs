using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Receipt : BaseEntity<int>
    {
        public Guid DonationId { get; set; }
        
        public Donation Donation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ReceiptNumber { get; set; } 
    }
}