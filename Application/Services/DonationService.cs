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
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class DonationService : IDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentGatewayService _paymentGateway;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public DonationService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentGatewayService paymentGateway, IHttpContextAccessor httpContextAccessor, INotificationService notificationService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentGateway = paymentGateway;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _configuration = configuration;
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

            // 3- check payment status first before saving anything
            if (paymentObj.Status == PaymentObjectStatus.Failed)
            {
                // Send error notification to admin
                await SendPaymentFailureNotificationToAdmin(donation, paymentObj);
                
                // Return error to user without saving to DB
                throw new InvalidOperationException("Failed to create payment link. Please try again later.");
            }

            // 4- only save to DB if payment object was created successfully
            donation.PaymentId = paymentObj.PaymentId;
            donation.Status = DonationStatus.Pending; // Keep as pending until webhook confirms

            await _unitOfWork.Donations.AddAsync(donation);
            await _unitOfWork.CompleteAsync();

            // 5- return successful response
            return new DonationPaymentResultDto
            {
                PaymentUrl = paymentObj.PaymentUrl,
                PaymentId = paymentObj.PaymentId,
                Status = paymentObj.Status.ToString()
            };
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
     
            return result;
        }

        public async Task<IEnumerable<RecentDonorDto>> GetRecentDonorsAsync(int count = 5, int? campaignId = null)
        {
   
            var baseQuery = _unitOfWork.Donations.GetQueryable()
                .Where(d => d.Status == DonationStatus.Successful);

    
            if (campaignId.HasValue)
            {
                baseQuery = baseQuery.Where(d => d.CampaignId == campaignId.Value);
            }

            var query = baseQuery.Include(d => d.User);

      
            var topDonors = await query
                .GroupBy(d => new { 
                    UserId = d.User != null ? d.User.Id : (Guid?)null,
                    DisplayName = d.User != null ? d.User.DisplayName : null,
                    UserName = d.User != null ? d.User.UserName : null,
                    IsAnonymous = d.IsAnonymous
                })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    Name = g.Key.IsAnonymous ?
                           "Anonymous Donor" :
                           (!string.IsNullOrWhiteSpace(g.Key.DisplayName) ? g.Key.DisplayName : g.Key.UserName),
                    TotalAmount = g.Sum(x => x.Amount),
                    IsAnonymous = g.Key.IsAnonymous
                })
                .OrderByDescending(d => d.TotalAmount)
                .Take(20) 
                .ToListAsync();

            
            var randomDonors = topDonors
                .OrderBy(_ => Guid.NewGuid())
                .Take(count)
                .Select(d => new RecentDonorDto
                {
                    Name = d.Name,
                    Amount = d.TotalAmount
                })
                .ToList();

            return randomDonors;
        }

        public async Task<IEnumerable<DonationResultDto>> GetDonationsByCampaignAsync(int campaignId)
        {
            var spec = new DonationsByCampaignSpecification(campaignId);
            var donations = await _unitOfWork.Donations.ListAsync(spec);
            var result = new List<DonationResultDto>();
            foreach (var donation in donations)
            {
                var dto = _mapper.Map<DonationResultDto>(donation);
          
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

        /// <summary>
        /// Send notification to admin when payment link creation fails
        /// </summary>
        private async Task SendPaymentFailureNotificationToAdmin(Donation donation, PaymentObjectDto paymentObj)
        {
            try
            {
                // Setup notification data
                var notificationData = new
                {
                    DonationId = donation.Id,
                    Amount = donation.Amount,
                    CampaignId = donation.CampaignId,
                    UserId = donation.UserId,
                    DonationDate = donation.DonationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    PaymentError = paymentObj.ErrorMessage ?? "Failed to create payment link",
                    PaymentGateway = "Stripe"
                };

                // Send notification to admin (admin email should be configured in settings)
                await _notificationService.CreateNotificationAsync(
                    NotificationTypeId.PaymentFailure, // This type should be added to enum
                    _configuration["AdminSettings:AdminEmail"] ?? "admin@linkdonation.com", // Read from configuration
                    notificationData,
                    NotificationLanguage.Arabic
                );

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                // Log error without breaking the main process
                // Can add logging here
                Console.WriteLine($"Failed to send payment failure notification: {ex.Message}");
            }
        }
    }
} 