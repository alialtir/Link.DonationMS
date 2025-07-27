using Application.Services.Abstractions;
using DTOs.DonationDTOs;
using DTOs.PaymentDTOs;
using Microsoft.Extensions.Configuration;
using Stripe;
using Domain.Contracts;
using AutoMapper;
using Domain.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
        }

        public async Task<PaymentIntentDto> CreateOrUpdatePaymentIntentAsync(CreateDonationDto createDonationDto)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(createDonationDto.Amount * 100), // Amount in cents
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };
            var intent = await service.CreateAsync(options);

            var donation = _mapper.Map<Donation>(createDonationDto);
            donation.DonationDate = DateTime.UtcNow;
            donation.Status = DonationStatus.Pending;
            donation.PaymentIntentId = intent.Id;
            donation.ClientSecret = intent.ClientSecret;

            await _unitOfWork.Donations.AddAsync(donation);
            await _unitOfWork.CompleteAsync();

            var resultDto = _mapper.Map<PaymentIntentDto>(intent);
            resultDto.DonationId = donation.Id;
            return resultDto;
        }

        public async Task<PaymentIntentDto> CreateOrUpdatePaymentIntentAsync(int donationId)
        {
            var donation = await _unitOfWork.Donations.GetByIdAsync(donationId);
            if (donation == null)
            {
                throw new KeyNotFoundException($"Donation with ID {donationId} was not found.");
            }

            var service = new PaymentIntentService();
            PaymentIntent intent;

            if (string.IsNullOrEmpty(donation.PaymentIntentId))
            {
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = (long)(donation.Amount * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(createOptions);
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions
                {
                    Amount = (long)(donation.Amount * 100)
                };
                intent = await service.UpdateAsync(donation.PaymentIntentId, updateOptions);
            }

            donation.PaymentIntentId = intent.Id;
            donation.ClientSecret = intent.ClientSecret;
            _unitOfWork.Donations.Update(donation);
            await _unitOfWork.CompleteAsync();

            var resultDto = _mapper.Map<PaymentIntentDto>(intent);
            resultDto.DonationId = donation.Id;
            return resultDto;
        }
    }
} 