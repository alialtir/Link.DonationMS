using Application.Services.Abstractions;
using DTOs.DonationDTOs;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using static Services.Specifications.DonationSpecifications;

namespace Services
{
    public class DonationService : IDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DonationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<DonationResultDto> CreateAsync(CreateDonationDto createDonationDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GenerateReceiptAsync(int donationId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DonationResultDto>> GetAllAsync(int pageNumber = 1)
        {
            int pageSize = 5;
            var spec = new DonationsWithPaginationSpecification(pageNumber, pageSize);
            var donations = await _unitOfWork.Donations.ListAsync(spec);
            var result = new List<DonationResultDto>();
            foreach (var donation in donations)
            {
                var dto = _mapper.Map<DonationResultDto>(donation);
                if (donation.Campaign != null)
                {
                    dto.CampaignTitle = !string.IsNullOrWhiteSpace(donation.Campaign.TitleAr)
                        ? donation.Campaign.TitleAr
                        : donation.Campaign.TitleEn;
                }
                if (donation.User != null)
                {
                    dto.DonorName = !string.IsNullOrWhiteSpace(donation.User.DisplayName)
                        ? donation.User.DisplayName
                        : donation.User.UserName;
                }
                result.Add(dto);
            }
            return result;
        }

        public Task<DonationResultDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DonationResultDto>> GetDonationsByCampaignAsync(int campaignId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DonationResultDto>> GetDonationsByUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalDonationsByCampaignAsync(int campaignId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ProcessPaymentAsync(int donationId, string paymentIntentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateDonationStatusAsync(int id, UpdateDonationStatusDto updateStatusDto)
        {
            throw new NotImplementedException();
        }
    }
} 