using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Persistence.Data;
using AuthServer.Services;
using Application.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OpenIddict.EntityFrameworkCore;

namespace AuthServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DonationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseOpenIddict<Guid>();
            });

            builder.Services.AddIdentity<User, Microsoft.AspNetCore.Identity.IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DonationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            //builder.Services.AddOpenIddict()
            //    .AddCore(options =>
            //    {
            //        options.UseEntityFrameworkCore()
            //               .UseDbContext<DonationDbContext>();
            //    })
            //    .AddServer(options =>
            //    {
            //        options
            //            .SetTokenEndpointUris("/connect/token");

            //        options
            //            .AllowPasswordFlow();

            //        options
            //            .AddDevelopmentEncryptionCertificate()
            //            .AddDevelopmentSigningCertificate();

            //        options
            //            .UseAspNetCore()
            //            .EnableTokenEndpointPassthrough();
            //    });

            //var jwtKey = builder.Configuration["Jwt:Key"];
            //var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            //var jwtAudience = builder.Configuration["Jwt:Audience"];

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = jwtIssuer,
            //        ValidAudience = jwtAudience,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "your-super-secret-key-with-at-least-256-bits"))
            //    };
            //});

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DonationDbContext>();
                context.Database.Migrate();
                await SeedDataAsync(context, scope.ServiceProvider);
            }

            app.Run();
        }

        private static async Task SeedDataAsync(DonationDbContext context, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>>();

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
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
