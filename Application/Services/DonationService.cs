using Application.Services.Abstractions;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using DTOs.DashboardDTOs;
using DTOs.DonationDTOs;
using DTOs.PaymentDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Services.Specifications.DonationSpecifications;
using Services.Resources;

namespace Services
{
    public class DonationService : IDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentGatewayService _paymentGateway;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DonationService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentGatewayService paymentGateway, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentGateway = paymentGateway;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DonationPaymentResultDto> CreateAsync(CreateDonationDto createDonationDto)
        {
         
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Guid? userId = null;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                userId = Guid.Parse(userIdClaim);
            }



            // 1- build donation object fully in memory (no DB save yet)
            var donation = _mapper.Map<Donation>(createDonationDto);
            donation.Id = Guid.NewGuid();
            donation.DonationDate = DateTime.UtcNow;
            donation.Status = DonationStatus.Pending;
            donation.UserId = userId;

            // 2- generate payment object (needs donation.Id & amount)
            var paymentObj = await _paymentGateway.GeneratePaymentObjectAsync(donation.Id, donation.Amount);

            // 3- attach payment info before first (and only) save
            donation.PaymentId = paymentObj.PaymentId;
            if (paymentObj.Status == PaymentObjectStatus.Failed)
            {
                donation.Status = DonationStatus.Failed;
            }

            // 4- save donation once
            await _unitOfWork.Donations.AddAsync(donation);
            await _unitOfWork.CompleteAsync();

            // 4- return unified response
            return new DonationPaymentResultDto
            {
                //DonationId = donation.Id,
                //Amount = donation.Amount,
                PaymentUrl = paymentObj.PaymentUrl,
                PaymentId = paymentObj.PaymentId,
                Status = paymentObj.Status.ToString()
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
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

        public async Task<DonationResultDto> GetByIdAsync(Guid id)
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

        public async Task<IEnumerable<RecentDonorDto>> GetRecentDonorsAsync(int count = 5, int? campaignId = null)
        {


            ISpecifications<Donation, Guid> spec = campaignId.HasValue
                ? new RecentRandomDonationsWithUserSpecification(campaignId.Value, count)
                : new RecentRandomDonationsWithUserSpecification(count);

            var donationsWithUsers = await _unitOfWork.Donations.ListAsync(spec);

            var result = new List<RecentDonorDto>();

            foreach (var donation in donationsWithUsers)
            {
                string donorName;
                bool isAnonymous = donation.IsAnonymous || donation.User == null;
                if (isAnonymous)
                {
                    donorName = PdfLabels.Anonymous;
                    result.Add(new RecentDonorDto { Name = donorName, Amount = donation.Amount });
                }
                else
                {
                    donorName = !string.IsNullOrWhiteSpace(donation.User.DisplayName)
                        ? donation.User.DisplayName
                        : donation.User.UserName;
                    var existing = result.FirstOrDefault(r => r.Name == donorName);
                    if (existing != null)
                    {
                        existing.Amount += donation.Amount;
                    }
                    else
                    {
                        result.Add(new RecentDonorDto { Name = donorName, Amount = donation.Amount });
                    }
                }
                if (result.Count >= count)
                    break;
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

        public async Task<bool> ProcessPaymentAsync(Guid donationId, string paymentIntentId)
        {
            // TODO: Stripe


            throw new NotImplementedException();
        }

        public async Task<bool> UpdateDonationStatusAsync(Guid id, UpdateDonationStatusDto updateStatusDto)
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