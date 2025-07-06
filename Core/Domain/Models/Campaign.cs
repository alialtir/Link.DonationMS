using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Campaign : BaseEntity<int>
    {
        [Required, MaxLength(200)]
        public string TitleAr { get; set; }

        [Required, MaxLength(200)]
        public string TitleEn { get; set; }

        [MaxLength(1000)]
        public string? DescriptionAr { get; set; }

        [MaxLength(1000)]
        public string? DescriptionEn { get; set; }

        [Required, Precision(18, 2)]
        public decimal GoalAmount { get; set; }

        [Precision(18, 2)]
        public decimal CurrentAmount { get; set; } = 0;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public CampaignStatus Status { get; set; } = CampaignStatus.Active;

        public byte[]? ImageData { get; set; }

        [MaxLength(10)]
        public string? ImageExtension { get; set; }

        public bool GoalReachedEmailSent { get; set; } = false;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<Donation> Donations { get; set; } = new List<Donation>();
    }
}
