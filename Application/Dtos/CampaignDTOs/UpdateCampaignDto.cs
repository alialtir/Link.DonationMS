using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.CampaignDTOs
{
    public class UpdateCampaignDto
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
        public decimal GoalAmount { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
