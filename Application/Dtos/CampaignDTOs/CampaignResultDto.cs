using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CampaignDTOs
{
    public class CampaignResultDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageExtension { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitleAr { get; set; }
        public string CategoryTitleEn { get; set; }
        public bool GoalReachedEmailSent { get; set; }
    }
}
