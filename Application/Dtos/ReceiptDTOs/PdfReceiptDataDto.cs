using System;

namespace DTOs.ReceiptDTOs
{
    /// <summary>
    /// DTO containing all data needed for PDF receipt generation
    /// </summary>
    public class PdfReceiptDataDto
    {
        public string ReceiptNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CampaignTitleEn { get; set; }
        public string CampaignTitleAr { get; set; }
        public decimal Amount { get; set; }
        public string DonorDisplayName { get; set; }
        public string DonorDisplayNameAr { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsArabic { get; set; }
    }
}
