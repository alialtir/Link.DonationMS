using Application.Services;
using Application.Services.Abstractions;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence.Data;


namespace Services
{
    public class ServiceManager : IServiceManager
    {
        public ServiceManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger<StripeGatewayService> logger,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            DonationDbContext context
        )
        {
            CampaignService = new CampaignService(unitOfWork, mapper, notificationService);
            // create payment gateway
            PaymentGatewayService = new StripeGatewayService(configuration, logger);
            DonationService = new DonationService(unitOfWork, mapper, PaymentGatewayService, httpContextAccessor);
            CategoryService = new CategoryService(unitOfWork, mapper);
            UserService = new UserService(unitOfWork, mapper, userManager, notificationService);
            NotificationService = new NotificationService(unitOfWork, mapper);
            ReceiptService = new ReceiptService(unitOfWork, mapper);
            DashboardService = new DashboardService(unitOfWork, mapper);
            AuthenticationService = new AuthenticationService(userManager, configuration);
         
        }

        public ICampaignService CampaignService { get; }
        public IDonationService DonationService { get; }
        public ICategoryService CategoryService { get; }
        public IUserService UserService { get; }
        public IDashboardService DashboardService { get; }
        public INotificationService NotificationService { get; }
        public IReceiptService ReceiptService { get; }
        public IPaymentGatewayService PaymentGatewayService { get; }
        public IAuthenticationService AuthenticationService { get; }
        public IEmailGatewayService EmailGatewayService { get; }
 
    }
} 