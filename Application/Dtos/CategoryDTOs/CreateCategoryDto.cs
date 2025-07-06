using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CategoryDTOs
{
    public class CreateCategoryDto
    {
        [Required, MaxLength(100)]
        public string TitleAr { get; set; }

        [Required, MaxLength(100)]
        public string TitleEn { get; set; }

        [MaxLength(1000)]
        public string? DescriptionAr { get; set; }

        [MaxLength(1000)]
        public string? DescriptionEn { get; set; }
    }
}
