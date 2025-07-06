namespace Application.Services.Abstractions
{
    public interface IServiceManager
    {
        ICampaignService CampaignService { get; }
        IDonationService DonationService { get; }
        ICategoryService CategoryService { get; }
        IUserService UserService { get; }
        IDashboardService DashboardService { get; }
        IEmailNotificationService EmailNotificationService { get; }
        IReceiptService ReceiptService { get; }
        IPaymentService PaymentService { get; }
        IAuthenticationService AuthenticationService { get; }
    }
} 