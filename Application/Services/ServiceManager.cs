using Application.Services.Abstractions;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        public ServiceManager(
            ICampaignService campaignService,
            IDonationService donationService,
            ICategoryService categoryService,
            IUserService userService,
            IDashboardService dashboardService,
            IEmailNotificationService emailNotificationService,
            IReceiptService receiptService,
            IPaymentService paymentService,
            IAuthenticationService authenticationService)
        {
            CampaignService = campaignService;
            DonationService = donationService;
            CategoryService = categoryService;
            UserService = userService;
            DashboardService = dashboardService;
            EmailNotificationService = emailNotificationService;
            ReceiptService = receiptService;
            PaymentService = paymentService;
            AuthenticationService = authenticationService;
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