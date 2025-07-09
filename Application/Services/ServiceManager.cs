using Application.Services.Abstractions;
using AutoMapper;
using Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        public ServiceManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<User> userManager,
            IConfiguration configuration
        )
        {
            CampaignService = new CampaignService(unitOfWork, mapper);
            DonationService = new DonationService(unitOfWork, mapper);
            CategoryService = new CategoryService(unitOfWork, mapper);
            UserService = new UserService(unitOfWork, mapper, userManager);
            DashboardService = new DashboardService();
            EmailNotificationService = new EmailNotificationService();
            ReceiptService = new ReceiptService();
            PaymentService = new PaymentService();
            AuthenticationService = new AuthenticationService(userManager, configuration);
        }

        public ICampaignService CampaignService { get; }
        public IDonationService DonationService { get; }
        public ICategoryService CategoryService { get; }
        public IUserService UserService { get; }
        public IDashboardService DashboardService { get; }
        public IEmailNotificationService EmailNotificationService { get; }
        public IReceiptService ReceiptService { get; }
        public IPaymentService PaymentService { get; }
        public IAuthenticationService AuthenticationService { get; }
    }
} 