using Microsoft.EntityFrameworkCore;
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

        [Required]
        [Precision(18, 2)]

        public decimal GoalAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public CampaignStatus Status { get; set; } = CampaignStatus.Active;

        public byte[]? ImageData { get; set; }

        public string? ImageExtension { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        //public Guid? UserId { get; set; }

        //public User User { get; set; }

        public bool GoalReachedEmailSent { get; set; } = false;


        public List<Donation> Donations { get; set; }


    }
}
