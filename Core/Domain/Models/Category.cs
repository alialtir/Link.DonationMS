using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Category : BaseEntity<int>
    {
        [Required, MaxLength(100)]
        public string TitleAr { get; set; }

        [Required, MaxLength(100)]
        public string TitleEn { get; set; }

        [MaxLength(1000)]
        public string? DescriptionAr { get; set; }

        [MaxLength(1000)]
        public string? DescriptionEn { get; set; }
        public List<Campaign> Campaigns { get; set; } 
    
    }
}