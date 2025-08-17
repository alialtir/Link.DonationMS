using Application.Services.Abstractions;
using DTOs.ReceiptDTOs;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using System.IO;
using System.Globalization;
using static Services.Constants.PdfConstants;
using Services.Resources;

namespace Services
{
    /// <summary>
    /// QuestPDF implementation of PDF generation gateway
    /// Handles all QuestPDF-specific logic
    /// </summary>
    public class QuestPdfGatewayService : IPdfGatewayService
    {
        public async Task<byte[]> GeneratePdfAsync(PdfReceiptDataDto receiptData)
        {
            return await Task.Run(() =>
            {
                using var stream = new MemoryStream();

                // Get localized labels
                string titleText = PdfLabels.Title;
                string receiptNumberLabel = PdfLabels.ReceiptNumber;
                string dateLabel = PdfLabels.Date;
                string campaignLabel = PdfLabels.Campaign;
                string amountLabel = PdfLabels.Amount;
                string donorLabel = PdfLabels.Donor;
                string anonymousText = PdfLabels.Anonymous;

                // Build PDF using QuestPDF
                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(PageSettings.PageMargin);
                        page.Content().Column(col =>
                        {
                            if (receiptData.IsArabic)
                            {
                                // Arabic layout
                                col.Item().AlignCenter().Text(titleText)
                                    .FontFamily(FontSettings.ArabicFontFamily)
                                    .FontSize(FontSettings.TitleFontSize)
                                    .Bold();

                                col.Item().AlignRight().Text($"{receiptNumberLabel}: {receiptData.ReceiptNumber}")
                                    .FontFamily(FontSettings.ArabicFontFamily)
                                    .FontSize(FontSettings.NormalFontSize);

                                col.Item().AlignRight().Text($"{dateLabel}: {receiptData.CreatedAt.ToString(ReceiptSettings.DateTimeFormat)}")
                                    .FontFamily(FontSettings.ArabicFontFamily);

                                col.Item().AlignRight().Text($"{campaignLabel}: {receiptData.CampaignTitleAr}")
                                    .FontFamily(FontSettings.ArabicFontFamily);

                                col.Item().AlignRight().Text($"{amountLabel}: {receiptData.Amount.ToString(ReceiptSettings.CurrencyFormat)}")
                                    .FontFamily(FontSettings.ArabicFontFamily);

                                var donorName = receiptData.IsAnonymous 
                                    ? anonymousText 
                                    : receiptData.DonorDisplayNameAr ?? receiptData.DonorDisplayName ?? anonymousText;

                                col.Item().AlignRight().Text($"{donorLabel}: {donorName}")
                                    .FontFamily(FontSettings.ArabicFontFamily);
                            }
                            else
                            {
                                // English layout
                                col.Item().AlignCenter().Text(titleText)
                                    .FontSize(FontSettings.TitleFontSize)
                                    .Bold();

                                col.Item().Text($"{receiptNumberLabel}: {receiptData.ReceiptNumber}")
                                    .FontSize(FontSettings.NormalFontSize);

                                col.Item().Text($"{dateLabel}: {receiptData.CreatedAt.ToString(ReceiptSettings.DateTimeFormat)}");

                                col.Item().Text($"{campaignLabel}: {receiptData.CampaignTitleEn}");

                                col.Item().Text($"{amountLabel}: {receiptData.Amount.ToString(ReceiptSettings.CurrencyFormat)}");

                                var donorName = receiptData.IsAnonymous 
                                    ? anonymousText 
                                    : receiptData.DonorDisplayName ?? anonymousText;

                                col.Item().Text($"{donorLabel}: {donorName}");
                            }
                        });
                    });
                });

                doc.GeneratePdf(stream);
                return stream.ToArray();
            });
        }
    }
}
