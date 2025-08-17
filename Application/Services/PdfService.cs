using Application.Services.Abstractions;
using DTOs.ReceiptDTOs;
using Domain.Contracts;
using Services.Specifications;
using System.Globalization;
using Services.Resources;
using static Services.Constants.PdfConstants;

namespace Services
{
    /// <summary>
    /// High-level PDF service that handles business logic
    /// Uses IPdfGatewayService for actual PDF generation
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPdfGatewayService _pdfGateway;

        public PdfService(IUnitOfWork unitOfWork, IPdfGatewayService pdfGateway)
        {
            _unitOfWork = unitOfWork;
            _pdfGateway = pdfGateway;
        }

        public async Task<byte[]> GenerateReceiptPdfAsync(int receiptId)
        {
            var receiptData = await GetReceiptDataAsync(receiptId);
            if (receiptData == null)
            {
                return null;
            }

            return await _pdfGateway.GeneratePdfAsync(receiptData);
        }

        /// <summary>
        /// Get receipt data from database and prepare for PDF generation
        /// </summary>
        private async Task<PdfReceiptDataDto> GetReceiptDataAsync(int receiptId)
        {
            // Use specification to fetch receipt with full details
            var spec = new ReceiptSpecifications.ReceiptWithDetailsSpecification(receiptId);
            var receipt = await _unitOfWork.Receipts.GetEntityWithSpecAsync(spec);
            
            if (receipt == null)
            {
                return null;
            }

            // Determine if current culture is Arabic
            var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == Languages.Arabic;

            // Map receipt to PDF data DTO
            return new PdfReceiptDataDto
            {
                ReceiptNumber = receipt.ReceiptNumber,
                CreatedAt = receipt.CreatedAt,
                CampaignTitleEn = receipt.Donation?.Campaign?.TitleEn,
                CampaignTitleAr = receipt.Donation?.Campaign?.TitleAr,
                Amount = receipt.Donation?.Amount ?? 0,
                DonorDisplayName = receipt.Donation?.User?.DisplayName,
                DonorDisplayNameAr = receipt.Donation?.User?.DisplayNameAr,
                IsAnonymous = receipt.Donation?.IsAnonymous ?? true,
                IsArabic = isArabic
            };
        }
    }
}
