namespace Application.Services.Abstractions
{
    public interface IServiceManager
    {
        ICampaignService CampaignService { get; }
        IDonationService DonationService { get; }
        ICategoryService CategoryService { get; }
        IUserService UserService { get; }
        IDashboardService DashboardService { get; }
        INotificationService NotificationService { get; }
        IReceiptService ReceiptService { get; }
        IPaymentGatewayService PaymentGatewayService { get; }
        IPdfGatewayService PdfGatewayService { get; }
        IPdfService PdfService { get; }
        IAuthenticationService AuthenticationService { get; }
    }
} 