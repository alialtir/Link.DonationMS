using Link.DonationMS.AdminPortal.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Link.DonationMS.AdminPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/Login"; // إعادة التوجيه لصفحة تسجيل الدخول عند رفض الوصول
                    //options.Cookie.SameSite = SameSiteMode.None;
                    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddGoogle("Google", options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                    //options.CallbackPath = "/Auth/GoogleCallback";
                    options.SaveTokens = true;

                    //// correlation cookie fix for HTTP localhost
                    //options.CorrelationCookie.SameSite = SameSiteMode.None;
                    //options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    //options.CorrelationCookie.HttpOnly = true;

                    options.Events.OnRemoteFailure = context => { return Task.CompletedTask; };
                });
            builder.Services.AddHttpContextAccessor();

            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                throw new InvalidOperationException("ApiSettings:BaseUrl is not configured in appsettings.json");
            }
            
            builder.Services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddScoped<ApiService>();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };

                foreach (var culture in supportedCultures)
                {
                    if (culture.Name.StartsWith("ar"))
                    {
                        culture.DateTimeFormat = new CultureInfo("en").DateTimeFormat;
                        culture.NumberFormat = new CultureInfo("en").NumberFormat;
                    }
                    
                    culture.NumberFormat.CurrencySymbol = "";
                }

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Use(async (context, next) =>
            {
                //if (context.User.Identity?.IsAuthenticated == true)
                //{
                //    // Allow both Admin and CampaignManager roles
                //    if (!context.User.IsInRole("Admin") && !context.User.IsInRole("CampaignManager"))
                //    {
                //        // User is authenticated but not authorized - redirect to unauthorized page
                //        context.Response.Redirect("/Home/Unauthorized");
                //        return;
                //    }
                //}
                await next();
            });

            app.Run();
        }
    }
}
