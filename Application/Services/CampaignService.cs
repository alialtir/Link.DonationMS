using Application.Services.Abstractions;
using DTOs.CampaignDTOs;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using static Services.Specifications.CampaignSpecifications;
using System.Linq;

namespace Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CampaignResultDto> CreateAsync(CreateCampaignDto createCampaignDto)
        {
            var campaign = _mapper.Map<Campaign>(createCampaignDto);
            campaign.Status = CampaignStatus.Active;
            campaign.StartDate = DateTime.UtcNow;
            await _unitOfWork.Campaigns.AddAsync(campaign);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.Categories.GetByIdAsync(campaign.CategoryId);
            return _mapper.Map<CampaignResultDto>(campaign);
        }

        public async Task<IEnumerable<CampaignResultDto>> GetAllAsync(int pageNumber = 1)
        {
            int pageSize = 5;
            var spec = new CampaignsWithPaginationSpecification(pageNumber, pageSize);
            var campaigns = await _unitOfWork.Campaigns.ListAsync(spec);
            var result = new List<CampaignResultDto>();
            var campaignsToUpdate = new List<Campaign>();
            
            foreach (var campaign in campaigns)
            {
                campaign.CurrentAmount = campaign.Donations?.Sum(d => d.Amount) ?? 0;
                
                var originalStatus = campaign.Status;
                
                if (campaign.CurrentAmount >= campaign.GoalAmount)
                {
                    campaign.Status = CampaignStatus.Completed;
                }
                else if (campaign.EndDate < DateTime.UtcNow)
                {
                    campaign.Status = CampaignStatus.Expired;
                }
                
                if (originalStatus != campaign.Status)
                {
                    campaignsToUpdate.Add(campaign);
                }
                
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
            
            if (campaignsToUpdate.Any())
            {
                foreach (var campaign in campaignsToUpdate)
                {
                    _unitOfWork.Campaigns.Update(campaign);
                }
                await _unitOfWork.CompleteAsync();
            }
            
            return result;
        }

        public async Task<CampaignResultDto> GetByIdAsync(int id)
        {
            var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaign == null) return null;
            
            campaign.CurrentAmount = campaign.Donations?.Sum(d => d.Amount) ?? 0;
            
            var originalStatus = campaign.Status;
            
            if (campaign.CurrentAmount >= campaign.GoalAmount)
            {
                campaign.Status = CampaignStatus.Completed;
            }
            else if (campaign.EndDate < DateTime.UtcNow)
            {
                campaign.Status = CampaignStatus.Expired;
            }
            
            if (originalStatus != campaign.Status)
            {
                _unitOfWork.Campaigns.Update(campaign);
                await _unitOfWork.CompleteAsync();
            }
            
            var dto = _mapper.Map<CampaignResultDto>(campaign);
            if (campaign.Category != null)
            {
                dto.CategoryTitleAr = campaign.Category.TitleAr;
                dto.CategoryTitleEn = campaign.Category.TitleEn;
            }
            return dto;
        }

        public async Task<CampaignResultDto> UpdateAsync(int id, UpdateCampaignDto updateCampaignDto)
        {
            var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaign == null) return null;
            _mapper.Map(updateCampaignDto, campaign);
            _unitOfWork.Campaigns.Update(campaign);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CampaignResultDto>(campaign);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaign == null) return false;
            _unitOfWork.Campaigns.Delete(campaign);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public Task<IEnumerable<CampaignResultDto>> GetActiveCampaignsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetCampaignProgressAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignResultDto>> GetCampaignsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignResultDto>> GetCompletedCampaignsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignResultDto>> SearchCampaignsByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }
    }
} 