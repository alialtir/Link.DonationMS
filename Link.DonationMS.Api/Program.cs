
using Application.Services;
using Application.Services.Abstractions;
using Application.Services.Resources;
using Domain.Contracts;
using Link.DonationMS.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Data;
using Services;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();





// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure supported cultures
var supportedCultures = new[] { "en", "ar" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[1]) // Arabic as default
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Link.DonationMS API", Version = "v1" });
});

// CORS configuration with specific allowed origins
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
    ?? new[] { "https://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("Content-Disposition");
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DefaultConnection is not configured in appsettings.json");
}

builder.Services.AddDbContext<DonationDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    // options.UseOpenIddict<Guid>();
});


builder.Services.AddIdentity<Domain.Models.User, Microsoft.AspNetCore.Identity.IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DonationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<Microsoft.AspNetCore.Identity.IdentityOptions>(options =>
{
    var config = builder.Configuration.GetSection("Identity");
    options.Password.RequireDigit = config.GetSection("Password:RequireDigit").Get<bool>();
    options.Password.RequireLowercase = config.GetSection("Password:RequireLowercase").Get<bool>();
    options.Password.RequireNonAlphanumeric = config.GetSection("Password:RequireNonAlphanumeric").Get<bool>();
    options.Password.RequireUppercase = config.GetSection("Password:RequireUppercase").Get<bool>();
    options.Password.RequiredLength = config.GetSection("Password:RequiredLength").Get<int>();
    options.Password.RequiredUniqueChars = config.GetSection("Password:RequiredUniqueChars").Get<int>();

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.Parse(config.GetSection("Lockout:DefaultLockoutTimeSpan").Value);
    options.Lockout.MaxFailedAccessAttempts = config.GetSection("Lockout:MaxFailedAccessAttempts").Get<int>();
    options.Lockout.AllowedForNewUsers = config.GetSection("Lockout:AllowedForNewUsers").Get<bool>();

    options.User.AllowedUserNameCharacters = config.GetSection("User:AllowedUserNameCharacters").Value;
    options.User.RequireUniqueEmail = config.GetSection("User:RequireUniqueEmail").Get<bool>();
});

var jwtKey = builder.Configuration["jwtOptions:SecretKey"];
var jwtIssuer = builder.Configuration["jwtOptions:Issuer"];
var jwtAudience = builder.Configuration["jwtOptions:Audience"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("jwtOptions:SecretKey is not configured");
}
if (string.IsNullOrEmpty(jwtIssuer))
{
    throw new InvalidOperationException("jwtOptions:Issuer is not configured");
}
if (string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("jwtOptions:Audience is not configured");
}



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
}).AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.SaveTokens = true;
}).AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    facebookOptions.SaveTokens = true;

    facebookOptions.Scope.Add("email");

});



builder.Services.AddAuthorization();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IPaymentGatewayService, StripeGatewayService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();



builder.Services.AddScoped<IStripeWebhookService, StripeWebhookService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfiles>());

var app = builder.Build();

app.UseHttpsRedirection();

// Apply localization middleware

// Ensure Accept-Language header has highest priority
localizationOptions.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use previously configured Cookie Policy settings
app.UseCookiePolicy();

app.UseCors("AllowAll");

// Disable HTTPS redirection in development to avoid OAuth issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DonationDbContext>();
        context.Database.Migrate();
        await SeedDataAsync(context, scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database or seeding data.");
        throw;
    }
}

app.Run();


static async Task SeedDataAsync(DonationDbContext context, IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Domain.Models.User>>();
    var roleManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>>();

    var roles = new[] { "Admin", "Donor", "CampaignManager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole<Guid>(role));
        }
    }

    var adminEmail = "admin@linkdonation.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new Domain.Models.User
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "System Administrator",
            DisplayNameAr = "مدير النظام",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Seed NotificationTypes
    //await SeedNotificationTypesAsync(context);
}

//static async Task SeedNotificationTypesAsync(DonationDbContext context)
//{
//    if (!context.NotificationTypes.Any())
//    {
//        var notificationTypes = new[]
//        {
//            // DonationReceipt notifications
//            new Domain.Models.NotificationType
//            {
//                TypeId = Domain.Models.NotificationTypeId.DonationReceipt,
//                Subject = "شكراً لتبرعك يا {{DonorName}}",
//                Body = "تم استلام تبرعك بمبلغ {{Amount}} لحملة {{CampaignName}}. رقم الإيصال: {{ReceiptNumber}}",
//                LanguageId = Domain.Models.NotificationLanguage.Arabic
//            },
//            new Domain.Models.NotificationType
//            {
//                TypeId = Domain.Models.NotificationTypeId.DonationReceipt,
//                Subject = "Thank you for your donation {{DonorName}}",
//                Body = "We have received your donation of {{Amount}} to {{CampaignName}}. Receipt No: {{ReceiptNumber}}",
//                LanguageId = Domain.Models.NotificationLanguage.English
//            },
            
//            // CampaignGoalReached notifications
//            new Domain.Models.NotificationType
//            {
//                TypeId = Domain.Models.NotificationTypeId.CampaignGoalReached,
//                Subject = "تم تحقيق هدف حملة {{CampaignName}}",
//                Body = "نود إبلاغك بأنه تم تحقيق هدف حملة {{CampaignName}}. شكراً لدعمكم الكريم",
//                LanguageId = Domain.Models.NotificationLanguage.Arabic
//            },
//            new Domain.Models.NotificationType
//            {
//                TypeId = Domain.Models.NotificationTypeId.CampaignGoalReached,
//                Subject = "Campaign {{CampaignName}} Goal Reached",
//                Body = "We are pleased to inform you that the campaign {{CampaignName}} has reached its funding goal. Thank you for your support!",
//                LanguageId = Domain.Models.NotificationLanguage.English
//            },
            
//            // Register notifications
//            new Domain.Models.NotificationType
//            {
//                TypeId = Domain.Models.NotificationTypeId.Register,
//                Subject = "مرحباً بك في منصة التبرعات",
//                Body = "مرحباً {{UserName}},\n\nشكراً لتسجيلك في منصة التبرعات. يمكنك الآن تسجيل الدخول والبدء في دعم الحملات المفضلة لديك.",
//                LanguageId = Domain.Models.NotificationLanguage.Arabic
//            },
//            new Domain.Models.NotificationType
//            {
//                TypeId = Domain.Models.NotificationTypeId.Register,
//                Subject = "Welcome to Donation Platform",
//                Body = "Hello {{UserName}},\n\nThank you for registering with our donation platform. You can now log in and start supporting your favorite campaigns.",
//                LanguageId = Domain.Models.NotificationLanguage.English
//            }
//        };

//        context.NotificationTypes.AddRange(notificationTypes);
//        await context.SaveChangesAsync();
//    }
//}


