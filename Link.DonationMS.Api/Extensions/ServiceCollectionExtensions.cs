using Application.Services;
using Application.Services.Abstractions;
using Domain.Contracts;
using Services;

namespace Link.DonationMS.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
        
     

            // Register all individual services (needed for ServiceManager constructor)
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<IPaymentGatewayService, StripeGatewayService>();
            services.AddScoped<IPdfGatewayService, QuestPdfGatewayService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Register ServiceManager (depends on all above services)
            services.AddScoped<IServiceManager, ServiceManager>();

            // Register additional services
            services.AddScoped<IStripeWebhookService, StripeWebhookService>();

            return services;
        }
    }
}
