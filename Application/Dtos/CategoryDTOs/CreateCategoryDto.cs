using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Resources;

namespace DTOs.CategoryDTOs
{
    public class CreateCategoryDto
    {
        [LocalizedRequired("TitleArRequired")]
        [LocalizedMaxLength(100, "TitleArMaxLength")]
        [LocalizedRegularExpression(@"^[\u0600-\u06FF\s]+$", "TitleArArabicOnly")]
        public string TitleAr { get; set; }

        [LocalizedRequired("TitleEnRequired")]
        [LocalizedMaxLength(100, "TitleEnMaxLength")]
        [LocalizedRegularExpression(@"^[a-zA-Z0-9\s\-\.]+$", "TitleEnEnglishOnly")]
        public string TitleEn { get; set; }

        [LocalizedMaxLength(1000, "DescriptionArMaxLength")]
        [LocalizedRegularExpression(@"^[\u0600-\u06FF\s\-\.\,\!\?\:]+$", "DescriptionArArabicOnly")]
        public string? DescriptionAr { get; set; }

        [LocalizedMaxLength(1000, "DescriptionEnMaxLength")]
        [LocalizedRegularExpression(@"^[a-zA-Z0-9\s\-\.\,\!\?\:]+$", "DescriptionEnEnglishOnly")]
        public string? DescriptionEn { get; set; }
    }
}
