using Application.Services.Abstractions;
using DTOs.CampaignDTOs;
using DTOs.DonationDTOs;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using static Services.Specifications.CampaignSpecifications;
using System.Linq;
using static Services.Specifications.DonationSpecifications;
using System;

namespace Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<CampaignResultDto> CreateAsync(CreateCampaignDto createCampaignDto)
        {
            var campaign = _mapper.Map<Campaign>(createCampaignDto);
            campaign.Status = CampaignStatus.Active;
            campaign.StartDate = DateTime.UtcNow;
            await _unitOfWork.Campaigns.AddAsync(campaign);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CampaignResultDto>(campaign);
        }

        public async Task<IEnumerable<CampaignResultDto>> GetAllAsync(int pageNumber = 1, int pageSize = 6)
        {
            var spec = new CampaignsWithPaginationSpecification(pageNumber, pageSize);
            var campaigns = await _unitOfWork.Campaigns.ListAsync(spec);
            var result = new List<CampaignResultDto>();
            foreach (var campaign in campaigns)
            {
                var successfulDonationsSpec = new SuccessfulDonationsByCampaignSpecification(campaign.Id);
                var successfulDonations = await _unitOfWork.Donations.ListAsync(successfulDonationsSpec);
                campaign.CurrentAmount = successfulDonations.Sum(d => d.Amount);
                var dto = _mapper.Map<CampaignResultDto>(campaign);
                if (campaign.Category != null)
                {
                    dto.CategoryTitleAr = !string.IsNullOrWhiteSpace(campaign.Category.TitleAr)
                        ? campaign.Category.TitleAr
                        : campaign.Category.TitleEn;
                    dto.CategoryTitleEn = !string.IsNullOrWhiteSpace(campaign.Category.TitleEn)
                        ? campaign.Category.TitleEn
                        : campaign.Category.TitleAr;
                }
                result.Add(dto);
            }
            return result;
        }

        public async Task<int> GetCountAsync()
        {
            var spec = new AllCampaignsSpecification();
            return await _unitOfWork.Campaigns.CountAsync(spec);
        }

        public async Task<CampaignResultDto> GetByIdAsync(int id)
        {
            var spec = new CampaignWithDetailsSpecification(id);
            var campaign = await _unitOfWork.Campaigns.GetEntityWithSpecAsync(spec);
            if (campaign == null) return null;
            var successfulDonationsSpec = new SuccessfulDonationsByCampaignSpecification(campaign.Id);
            var successfulDonations = await _unitOfWork.Donations.ListAsync(successfulDonationsSpec);
            campaign.CurrentAmount = successfulDonations.Sum(d => d.Amount);
            var dto = _mapper.Map<CampaignResultDto>(campaign);
            if (campaign.Category != null)
            {
                dto.CategoryTitleAr = !string.IsNullOrWhiteSpace(campaign.Category.TitleAr)
                    ? campaign.Category.TitleAr
                    : campaign.Category.TitleEn;
                dto.CategoryTitleEn = !string.IsNullOrWhiteSpace(campaign.Category.TitleEn)
                    ? campaign.Category.TitleEn
                    : campaign.Category.TitleAr;
            }
            return dto;
        }

        public async Task<CampaignResultDto> UpdateAsync(int id, UpdateCampaignDto updateCampaignDto)
        {
            var spec = new CampaignWithDetailsSpecification(id);

            var campaign = await _unitOfWork.Campaigns.GetEntityWithSpecAsync(spec);

            if (campaign == null) return null;

            _mapper.Map(updateCampaignDto, campaign);
            
            if (Enum.TryParse<CampaignStatus>(updateCampaignDto.Status, out var newStatus))
            {
                campaign.Status = newStatus;
            }

            _unitOfWork.Campaigns.Update(campaign);

            await _unitOfWork.CompleteAsync();

            var updatedCampaign = await _unitOfWork.Campaigns.GetEntityWithSpecAsync(spec);

            var dto = _mapper.Map<CampaignResultDto>(updatedCampaign);

            if (updatedCampaign.Category != null)
            {
                dto.CategoryTitleAr = !string.IsNullOrWhiteSpace(updatedCampaign.Category.TitleAr)
                    ? updatedCampaign.Category.TitleAr
                    : updatedCampaign.Category.TitleEn;
                dto.CategoryTitleEn = !string.IsNullOrWhiteSpace(updatedCampaign.Category.TitleEn)
                    ? updatedCampaign.Category.TitleEn
                    : updatedCampaign.Category.TitleAr;
            }
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaign == null) return false;
            _unitOfWork.Campaigns.Delete(campaign);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<decimal> GetCampaignProgressAsync(int id)
        {
            var spec = new CampaignWithDetailsSpecification(id);
            var campaign = await _unitOfWork.Campaigns.GetEntityWithSpecAsync(spec);
            if (campaign == null) return 0;
            
            var successfulDonationsSpec = new SuccessfulDonationsByCampaignSpecification(id);
            var successfulDonations = await _unitOfWork.Donations.ListAsync(successfulDonationsSpec);
            var totalRaised = successfulDonations.Sum(d => d.Amount);
            
            if (campaign.GoalAmount <= 0) return 0;
            var percentage = (totalRaised / campaign.GoalAmount) * 100;
            return Math.Min(percentage, 100);
        }

        public async Task<IEnumerable<CampaignResultDto>> GetCampaignsByCategoryAsync(int categoryId)
        {
            var spec = new CampaignsByCategorySpecification(categoryId);
            var campaigns = await _unitOfWork.Campaigns.ListAsync(spec);
            var result = new List<CampaignResultDto>();
            foreach (var campaign in campaigns)
            {
                var successfulDonationsSpec = new SuccessfulDonationsByCampaignSpecification(campaign.Id);
                var successfulDonations = await _unitOfWork.Donations.ListAsync(successfulDonationsSpec);
                campaign.CurrentAmount = successfulDonations.Sum(d => d.Amount);
                var dto = _mapper.Map<CampaignResultDto>(campaign);
                if (campaign.Category != null)
                {
                    dto.CategoryTitleAr = !string.IsNullOrWhiteSpace(campaign.Category.TitleAr)
                        ? campaign.Category.TitleAr
                        : campaign.Category.TitleEn;
                    dto.CategoryTitleEn = !string.IsNullOrWhiteSpace(campaign.Category.TitleEn)
                        ? campaign.Category.TitleEn
                        : campaign.Category.TitleAr;
                }
                result.Add(dto);
            }
            return result;
        }

        public async Task<IEnumerable<CampaignResultDto>> GetCompletedCampaignsAsync()
        {
            var spec = new CompletedCampaignsSpecification();
            var campaigns = await _unitOfWork.Campaigns.ListAsync(spec);
            var result = new List<CampaignResultDto>();
            foreach (var campaign in campaigns)
            {
                var successfulDonationsSpec = new SuccessfulDonationsByCampaignSpecification(campaign.Id);
                var successfulDonations = await _unitOfWork.Donations.ListAsync(successfulDonationsSpec);
                campaign.CurrentAmount = successfulDonations.Sum(d => d.Amount);
                var dto = _mapper.Map<CampaignResultDto>(campaign);
                if (campaign.Category != null)
                {
                    dto.CategoryTitleAr = !string.IsNullOrWhiteSpace(campaign.Category.TitleAr)
                        ? campaign.Category.TitleAr
                        : campaign.Category.TitleEn;
                    dto.CategoryTitleEn = !string.IsNullOrWhiteSpace(campaign.Category.TitleEn)
                        ? campaign.Category.TitleEn
                        : campaign.Category.TitleAr;
                }
                result.Add(dto);
            }
            return result;
        }

        public async Task<IEnumerable<CampaignResultDto>> SearchCampaignsByTitleAsync(string title)
        {
            var spec = new CampaignsByTitleSpecification(title);
            var campaigns = await _unitOfWork.Campaigns.ListAsync(spec);
            var result = new List<CampaignResultDto>();
            foreach (var campaign in campaigns)
            {
                var successfulDonationsSpec = new SuccessfulDonationsByCampaignSpecification(campaign.Id);
                var successfulDonations = await _unitOfWork.Donations.ListAsync(successfulDonationsSpec);
                campaign.CurrentAmount = successfulDonations.Sum(d => d.Amount);
                var dto = _mapper.Map<CampaignResultDto>(campaign);
                if (campaign.Category != null)
                {
                    dto.CategoryTitleAr = !string.IsNullOrWhiteSpace(campaign.Category.TitleAr)
                        ? campaign.Category.TitleAr
                        : campaign.Category.TitleEn;
                    dto.CategoryTitleEn = !string.IsNullOrWhiteSpace(campaign.Category.TitleEn)
                        ? campaign.Category.TitleEn
                        : campaign.Category.TitleAr;
                }
                result.Add(dto);
            }
            return result;
        }

        public async Task<IEnumerable<CampaignResultDto>> GetActiveCampaignsFilteredAsync(string title = null, int? categoryId = null, int pageNumber = 1, int pageSize = 5)
        {
            var spec = new ActiveCampaignsFilteredSpecification(title, categoryId);
            var campaigns = await _unitOfWork.Campaigns.ListAsync(spec);
            var result = new List<CampaignResultDto>();
            foreach (var campaign in campaigns)
            {
                var successfulDonationsSpec = new SuccessfulDonationsByCampaignSpecification(campaign.Id);
                var successfulDonations = await _unitOfWork.Donations.ListAsync(successfulDonationsSpec);
                campaign.CurrentAmount = successfulDonations.Sum(d => d.Amount);
                var dto = _mapper.Map<CampaignResultDto>(campaign);
                if (campaign.Category != null)
                {
                    dto.CategoryTitleAr = !string.IsNullOrWhiteSpace(campaign.Category.TitleAr) ? campaign.Category.TitleAr : campaign.Category.TitleEn;
                    dto.CategoryTitleEn = !string.IsNullOrWhiteSpace(campaign.Category.TitleEn) ? campaign.Category.TitleEn : campaign.Category.TitleAr;
                }
                result.Add(dto);
            }
            return result;
        }

        public async Task UpdateCampaignProgressAsync(DTOs.DonationDTOs.DonationProgressDto dto)
        {
            var campaign = await _unitOfWork.Campaigns.GetByIdAsync(dto.CampaignId);
            if (campaign == null) return;

            var wasGoalReached = campaign.CurrentAmount >= campaign.GoalAmount;
            campaign.CurrentAmount += dto.Amount;

            if (campaign.Status == CampaignStatus.Active &&
                campaign.CurrentAmount >= campaign.GoalAmount &&
                !wasGoalReached) // Only create notification if goal just reached
            {
                campaign.Status = CampaignStatus.Completed;
                
                // Create campaign goal reached notification
                try
                {
await _notificationService.CreateNotificationAsync(
                        NotificationTypeId.CampaignGoalReached,
                        "admin@linkdonation.com", // Admin email - يمكن ضبطه
                        new
                        {
                            CampaignName = !string.IsNullOrWhiteSpace(campaign.TitleAr) ? campaign.TitleAr : campaign.TitleEn
                        },
                        NotificationLanguage.Arabic 
                    );
                }
                catch (Exception ex)
                {
                    // Log error but don't break the donation process
                    // You can add logging here if needed
                }
            }

            _unitOfWork.Campaigns.Update(campaign);
        }


    }
} 