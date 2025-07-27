using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Resources;

namespace DTOs.CampaignDTOs
{
    public class CreateCampaignDto
    {
        [LocalizedRequired("TitleArRequired")]
        [LocalizedMaxLength(200, "TitleArMaxLength")]
        [LocalizedRegularExpression(@"^[\u0600-\u06FF\s]+$", "TitleArArabicOnly")]
        public string TitleAr { get; set; }

        [LocalizedRequired("TitleEnRequired")]
        [LocalizedMaxLength(200, "TitleEnMaxLength")]
        [LocalizedRegularExpression(@"^[a-zA-Z0-9\s\-\.]+$", "TitleEnEnglishOnly")]
        public string TitleEn { get; set; }

        [LocalizedMaxLength(1000, "DescriptionArMaxLength")]
        [LocalizedRegularExpression(@"^[\u0600-\u06FF\s\-\.\,\!\?\:]*$", "DescriptionArArabicOnly")]
        public string? DescriptionAr { get; set; }

        [LocalizedMaxLength(1000, "DescriptionEnMaxLength")]
        [LocalizedRegularExpression(@"^[a-zA-Z0-9\s\-\.\,\!\?\:]*$", "DescriptionEnEnglishOnly")]
        public string? DescriptionEn { get; set; }

        [LocalizedRequired("GoalAmountRequired")]
        [LocalizedRange(0.01, double.MaxValue, "GoalAmountGreaterThanZero")]
        [LocalizedRegularExpression(@"^[0-9]+(\.[0-9]{1,2})?$", "GoalAmountValidNumber")]
        public decimal GoalAmount { get; set; }

        [LocalizedRequired("EndDateRequired")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [LocalizedRequired("CategoryRequired")]
        [LocalizedRange(1, int.MaxValue, "ValidCategory")]
        public int CategoryId { get; set; }

        public byte[]? ImageData { get; set; }

        [LocalizedMaxLength(10, "ImageExtensionMaxLength")]
        [LocalizedRegularExpression(@"^[a-zA-Z0-9]+$", "ImageExtensionLettersNumbersOnly")]
        public string? ImageExtension { get; set; }
    }
}
