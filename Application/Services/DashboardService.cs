using Application.Services.Abstractions;

using AutoMapper;
using Services.Specifications;
using Domain.Contracts;
using Domain.Models;
using DTOs.DashboardDTOs;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<CampaignProgressDto>> GetTopCampaignsAsync()
        {
            // Get top 5 campaigns ordered by the amount collected so far (CurrentAmount)
            const int TOP_COUNT = 5;
            var spec = new CampaignSpecifications.TopCampaignsSpecification(TOP_COUNT);
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