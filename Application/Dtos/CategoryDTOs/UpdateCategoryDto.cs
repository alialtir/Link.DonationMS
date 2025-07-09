using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.CategoryDTOs
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Title in Arabic is required")]
        [MaxLength(100, ErrorMessage = "Title in Arabic cannot exceed 100 characters")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "Title in Arabic can only contain Arabic letters and spaces")]
        public string TitleAr { get; set; }

        [Required(ErrorMessage = "Title in English is required")]
        [MaxLength(100, ErrorMessage = "Title in English cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.]+$", ErrorMessage = "Title in English can only contain English letters, numbers, spaces, hyphens, and dots")]
        public string TitleEn { get; set; }

        [MaxLength(1000, ErrorMessage = "Description in Arabic cannot exceed 1000 characters")]
        [RegularExpression(@"^[\u0600-\u06FF\s\-\.\,\!\?\:]+$", ErrorMessage = "Description in Arabic can only contain Arabic letters, spaces, and common punctuation")]
        public string? DescriptionAr { get; set; }

        [MaxLength(1000, ErrorMessage = "Description in English cannot exceed 1000 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.\,\!\?\:]+$", ErrorMessage = "Description in English can only contain English letters, numbers, spaces, and common punctuation")]
        public string? DescriptionEn { get; set; }
    }
}
