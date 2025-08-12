using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services.Abstractions;
using DTOs.ReceiptDTOs;
using AutoMapper;
using System.Linq;
using Services.Specifications;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using System.IO;
using System.Globalization;
using static Services.Constants.PdfConstants;
using Services.Resources;

using Domain.Contracts;
using Domain.Models;

namespace Services
{
    public class ReceiptService : IReceiptService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> GenerateReceiptAsync(Guid donationId)
        {
            var receipt = new Receipt
            {
                DonationId = donationId,
                ReceiptNumber = Guid.NewGuid().ToString("N").Substring(0, ReceiptSettings.ReceiptNumberLength)
            };
            await _unitOfWork.Receipts.AddAsync(receipt);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<byte[]> GeneratePdfAsync(int receiptId)
        {
            // Use specification to fetch receipt with full details
            var spec = new ReceiptSpecifications.ReceiptWithDetailsSpecification(receiptId);
            var receipt = await _unitOfWork.Receipts.GetEntityWithSpecAsync(spec);
            if (receipt == null)
            {
                return null;
            }

            // Set culture for resource localization
var currentCulture = CultureInfo.CurrentUICulture;
var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == Languages.Arabic;



string titleText = PdfLabels.Title;
string receiptNumberLabel = PdfLabels.ReceiptNumber;
string dateLabel = PdfLabels.Date;
string campaignLabel = PdfLabels.Campaign;
string amountLabel = PdfLabels.Amount;
string donorLabel = PdfLabels.Donor;
string anonymousText = PdfLabels.Anonymous;

// Build simple PDF using QuestPDF
            using var stream = new MemoryStream();

            
            var doc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(PageSettings.PageMargin);
                    page.Content().Column(col =>
                    {
                        if (isArabic)
                        {
                            col.Item().AlignCenter().Text(titleText).FontFamily(FontSettings.ArabicFontFamily).FontSize(FontSettings.TitleFontSize).Bold();
                            col.Item().AlignRight().Text($"{receiptNumberLabel}: {receipt.ReceiptNumber}").FontFamily(FontSettings.ArabicFontFamily).FontSize(FontSettings.NormalFontSize);
                            col.Item().AlignRight().Text($"{dateLabel}: {receipt.CreatedAt.ToString(ReceiptSettings.DateTimeFormat)}").FontFamily(FontSettings.ArabicFontFamily);
                            col.Item().AlignRight().Text($"{campaignLabel}: {receipt.Donation.Campaign.TitleAr}").FontFamily(FontSettings.ArabicFontFamily);
                            col.Item().AlignRight().Text($"{amountLabel}: {receipt.Donation.Amount.ToString(ReceiptSettings.CurrencyFormat)}").FontFamily(FontSettings.ArabicFontFamily);
                            col.Item().AlignRight().Text($"{donorLabel}: {receipt.Donation.User?.DisplayNameAr ?? anonymousText}").FontFamily(FontSettings.ArabicFontFamily);
                        }
                        else
                        {
                            col.Item().AlignCenter().Text(titleText).FontSize(FontSettings.TitleFontSize).Bold();
                            col.Item().Text($"{receiptNumberLabel}: {receipt.ReceiptNumber}").FontSize(FontSettings.NormalFontSize);
                            col.Item().Text($"{dateLabel}: {receipt.CreatedAt.ToString(ReceiptSettings.DateTimeFormat)}");
                            col.Item().Text($"{campaignLabel}: {receipt.Donation.Campaign.TitleEn}");
                            col.Item().Text($"{amountLabel}: {receipt.Donation.Amount.ToString(ReceiptSettings.CurrencyFormat)}");
                            col.Item().Text($"{donorLabel}: {receipt.Donation.User?.DisplayName ?? anonymousText}");
                        }
                    });
                });
            });

            doc.GeneratePdf(stream);
            return stream.ToArray();
        }

        public async Task<ReceiptDto> GetByDonationIdAsync(Guid donationId)
        {
            var spec = new ReceiptSpecifications.ReceiptByDonationSpecification(donationId);
            var receipt = await _unitOfWork.Receipts.GetEntityWithSpecAsync(spec);
            if (receipt == null) return null;
            return new ReceiptDto
            {
                Id = receipt.Id,
                DonationId = receipt.DonationId,
                CreatedAt = receipt.CreatedAt,
                Amount = receipt.Donation?.Amount ?? 0,
                CampaignId = receipt.Donation?.CampaignId ?? (receipt.Donation?.Campaign?.Id != null ? (int?)receipt.Donation.Campaign.Id : null),
                CampaignTitleAr = receipt.Donation?.Campaign?.TitleAr,
                CampaignTitleEn = receipt.Donation?.Campaign?.TitleEn,
                ReceiptNumber = receipt.ReceiptNumber
            };
        }

        public async Task<ReceiptDto> GetByIdAsync(int id)
        {
            var spec = new ReceiptSpecifications.ReceiptWithDetailsSpecification(id);
            var receipt = await _unitOfWork.Receipts.GetEntityWithSpecAsync(spec);
            if (receipt == null) return null;
            return new ReceiptDto
            {
                Id = receipt.Id,
                DonationId = receipt.DonationId,
                CreatedAt = receipt.CreatedAt,
                Amount = receipt.Donation?.Amount ?? 0,
                CampaignId = receipt.Donation?.CampaignId ?? (receipt.Donation?.Campaign?.Id != null ? (int?)receipt.Donation.Campaign.Id : null),
                CampaignTitleAr = receipt.Donation?.Campaign?.TitleAr,
                CampaignTitleEn = receipt.Donation?.Campaign?.TitleEn,
                ReceiptNumber = receipt.ReceiptNumber
            };
        }

        public async Task<IEnumerable<ReceiptDto>> GetByUserAsync(Guid userId)
        {
            // Retrieve all receipts then filter by donation's userId
            var spec = new ReceiptSpecifications.ReceiptsByUserSpecification(userId);
            var userReceipts = await _unitOfWork.Receipts.ListAsync(spec);
            // Map to DTO including amount and campaign title from related entities
            var result = userReceipts.Select(r => new ReceiptDto
            {
                Id = r.Id,
                DonationId = r.DonationId,
                CreatedAt = r.CreatedAt,
                Amount = r.Donation.Amount,
                CampaignId = r.Donation.CampaignId,
                CampaignTitleAr = r.Donation.Campaign?.TitleAr,
                CampaignTitleEn = r.Donation.Campaign?.TitleEn,
                ReceiptNumber = r.ReceiptNumber
            });
            return result;
        }

        public Task<bool> SendEmailAsync(int receiptId, string email)
        {
            throw new NotImplementedException();
        }
    }
} 