using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Campaign : BaseEntity<int>
    {
        [Required, MaxLength(100)]
        public string TitleAr { get; set; }

        [Required, MaxLength(100)]
        public string TitleEn { get; set; }

        [MaxLength(1000)]

        public string? DescriptionAr { get; set; }

        [MaxLength(1000)]
        public string? DescriptionEn { get; set; }

        [Required, Range(1,100000)]

        public decimal GoalAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public CampaignStatus Status { get; set; } = CampaignStatus.Active;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public List<Donation> Donations { get; set; }


    }
}
