using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services.Abstractions;
using DTOs.ReceiptDTOs;
using AutoMapper;
using System.Linq;
using Services.Specifications;
using Domain.Contracts;
using Domain.Models;
using static Services.Constants.PdfConstants;

namespace Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPdfService _pdfService;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper, IPdfService pdfService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pdfService = pdfService;
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
            // Delegate PDF generation to the PDF service
            return await _pdfService.GenerateReceiptPdfAsync(receiptId);
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