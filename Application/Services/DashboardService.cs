using Application.Services.Abstractions;

using AutoMapper;
using Services.Specifications;
using Domain.Contracts;
using Domain.Models;
using DTOs.DashboardDTOs;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }


        public async Task<IEnumerable<CampaignProgressDto>> GetTopCampaignsAsync(int? count = null)
        {
            // Get count from parameter, or from appsettings.json, or default to 5
            int topCount = count ?? _configuration.GetValue<int>("Dashboard:TopCampaignsCount", 5);
            
            var spec = new CampaignSpecifications.TopCampaignsSpecification(topCount);
            var campaigns = await _unitOfWork.Campaigns.ListAsync(spec);

            // Project to DTO
            var dtoList = campaigns.Select(c => new CampaignProgressDto
            {
                Id = c.Id,
                TitleAr = c.TitleAr,
                TitleEn = c.TitleEn,
                GoalAmount = c.GoalAmount,
                CurrentAmount = c.CurrentAmount,
                ProgressPercentage = c.GoalAmount == 0 ? 0 : Math.Round((c.CurrentAmount / c.GoalAmount) * 100, 2),
                Status = c.Status.ToString()
            });

            return dtoList;
        }

        public async Task<decimal> GetTotalDonationsAsync()
        {
            var successfulDonations = await _unitOfWork.Donations.ListAsync(new DonationSpecifications.SuccessfulDonationsSpecification());
            return successfulDonations.Sum(d => d.Amount);
        }

        public async Task<int> GetTotalDonorsAsync()
        {
            var successfulDonations = await _unitOfWork.Donations.ListAsync(new DonationSpecifications.SuccessfulDonationsSpecification());
            var distinctDonors = successfulDonations
                .Where(d => d.UserId.HasValue)
                .Select(d => d.UserId!.Value)
                .Distinct()
                .Count();
            return distinctDonors;
        }
    }
}