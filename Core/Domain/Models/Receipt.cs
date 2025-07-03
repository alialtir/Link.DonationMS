using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Receipt : BaseEntity<int>
    {
        public int DonationId { get; set; }
        
        public Donation Donation { get; set; }

 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}