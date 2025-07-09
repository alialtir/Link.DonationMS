using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.CampaignDTOs
{
    public class UpdateCampaignDto
    {
        [Required(ErrorMessage = "Title in Arabic is required")]
        [MaxLength(200, ErrorMessage = "Title in Arabic cannot exceed 200 characters")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "Title in Arabic can only contain Arabic letters and spaces")]
        public string TitleAr { get; set; }

        [Required(ErrorMessage = "Title in English is required")]
        [MaxLength(200, ErrorMessage = "Title in English cannot exceed 200 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.]+$", ErrorMessage = "Title in English can only contain English letters, numbers, spaces, hyphens, and dots")]
        public string TitleEn { get; set; }

        [MaxLength(1000, ErrorMessage = "Description in Arabic cannot exceed 1000 characters")]
        [RegularExpression(@"^[\u0600-\u06FF\s\-\.\,\!\?\:]*$", ErrorMessage = "Description in Arabic can only contain Arabic letters, spaces, and common punctuation")]
        public string? DescriptionAr { get; set; }

        [MaxLength(1000, ErrorMessage = "Description in English cannot exceed 1000 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.\,\!\?\:]*$", ErrorMessage = "Description in English can only contain English letters, numbers, spaces, and common punctuation")]
        public string? DescriptionEn { get; set; }

        [Required(ErrorMessage = "Goal amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Goal amount must be greater than 0")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})?$", ErrorMessage = "Goal amount must be a valid number with up to 2 decimal places")]
        public decimal GoalAmount { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        public byte[]? ImageData { get; set; }

        [MaxLength(10, ErrorMessage = "Image extension cannot exceed 10 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Image extension can only contain letters and numbers")]
        public string? ImageExtension { get; set; }
    }
}
