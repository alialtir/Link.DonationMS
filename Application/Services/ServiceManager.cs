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
            INotificationService notificationService,
            IReceiptService receiptService,
            IPaymentGatewayService paymentGatewayService,
            IPdfGatewayService pdfGatewayService,
            IPdfService pdfService,
            IAuthenticationService authenticationService
        )
        {
            CampaignService = campaignService;
            DonationService = donationService;
            CategoryService = categoryService;
            UserService = userService;
            DashboardService = dashboardService;
            NotificationService = notificationService;
            ReceiptService = receiptService;
            PaymentGatewayService = paymentGatewayService;
            PdfGatewayService = pdfGatewayService;
            PdfService = pdfService;
            AuthenticationService = authenticationService;
        }

        public ICampaignService CampaignService { get; }
        public IDonationService DonationService { get; }
        public ICategoryService CategoryService { get; }
        public IUserService UserService { get; }
        public IDashboardService DashboardService { get; }
        public INotificationService NotificationService { get; }
        public IReceiptService ReceiptService { get; }
        public IPaymentGatewayService PaymentGatewayService { get; }
        public IPdfGatewayService PdfGatewayService { get; }
        public IPdfService PdfService { get; }
        public IAuthenticationService AuthenticationService { get; }
    }
} 