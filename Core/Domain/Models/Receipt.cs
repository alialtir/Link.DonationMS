using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Receipt : BaseEntity<int>
    {
        public int DonationId { get; set; }
        
        public Donation Donation { get; set; }

        [Required, MaxLength(500)]
        public string FilePath { get; set; } 
    }
}