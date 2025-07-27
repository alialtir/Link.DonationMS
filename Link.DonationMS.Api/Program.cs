
using Link.DonationMS.Api;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Application.Services.Abstractions;
using Services;
using Persistence;
using Domain.Contracts;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Link.DonationMS API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
.AddJwtBearer(options =>
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
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfiles>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

app.UseHttpsRedirection();

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
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }


}
