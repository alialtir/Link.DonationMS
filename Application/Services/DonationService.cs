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

        public async Task<DonationResultDto> CreateAsync(CreateDonationDto createDonationDto)
        {

            var donation = _mapper.Map<Donation>(createDonationDto);
            donation.DonationDate = DateTime.UtcNow;
            donation.Status = DonationStatus.Pending;
            await _unitOfWork.Donations.AddAsync(donation);
            await _unitOfWork.CompleteAsync();
            var result = _mapper.Map<DonationResultDto>(donation);
            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var spec = new DonationWithDetailsSpecification(id);
            var donation = await _unitOfWork.Donations.GetEntityWithSpecAsync(spec);
            if (donation == null) return false;
            _unitOfWork.Donations.Delete(donation);
            await _unitOfWork.CompleteAsync();
            return true;
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

        public async Task<DonationResultDto> GetByIdAsync(int id)
        {
            var spec = new DonationWithDetailsSpecification(id);
            var donation = await _unitOfWork.Donations.GetEntityWithSpecAsync(spec);
            if (donation == null) return null;
            var result = _mapper.Map<DonationResultDto>(donation);
            if (donation.Campaign != null)
            {
                result.CampaignTitle = !string.IsNullOrWhiteSpace(donation.Campaign.TitleAr)
                    ? donation.Campaign.TitleAr
                    : donation.Campaign.TitleEn;
            }
            if (donation.User != null)
            {
                result.DonorName = !string.IsNullOrWhiteSpace(donation.User.DisplayName)
                    ? donation.User.DisplayName
                    : donation.User.UserName;
            }
            return result;
        }

        public async Task<IEnumerable<DonationResultDto>> GetDonationsByCampaignAsync(int campaignId)
        {
            var spec = new DonationsByCampaignSpecification(campaignId);
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

        public async Task<IEnumerable<DonationResultDto>> GetDonationsByUserAsync(Guid userId)
        {
            var spec = new DonationsByUserSpecification(userId);
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

        public async Task<decimal> GetTotalDonationsByCampaignAsync(int campaignId)
        {
            var spec = new SuccessfulDonationsByCampaignSpecification(campaignId);
            var donations = await _unitOfWork.Donations.ListAsync(spec);
            return donations.Sum(d => d.Amount);
        }

        public async Task<bool> ProcessPaymentAsync(int donationId, string paymentIntentId)
        {
            // TODO: Stripe


            throw new NotImplementedException();
        }

        public async Task<bool> UpdateDonationStatusAsync(int id, UpdateDonationStatusDto updateStatusDto)
        {
            var spec = new DonationWithDetailsSpecification(id);
            var donation = await _unitOfWork.Donations.GetEntityWithSpecAsync(spec);
            if (donation == null) return false;
            
        
            if (Enum.TryParse<DonationStatus>(updateStatusDto.Status, out var status))
            {
                donation.Status = status;
                _unitOfWork.Donations.Update(donation);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            
            return false;
        }
    }
} 