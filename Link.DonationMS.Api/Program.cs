
using Link.DonationMS.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Domain.Models;
using Application.Services.Abstractions;
using Services;
using Persistence;
using Microsoft.OpenApi.Models;
using Domain.Contracts;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DonationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DonationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<DonationDbContext>();
    })
    .AddServer(options =>
    {
        options
            .SetTokenEndpointUris("/connect/token")
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey()
            .DisableAccessTokenEncryption()
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "https://localhost:7119";
    options.Audience = "api";
    options.RequireHttpsMetadata = false;
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<Application.Services.Abstractions.IAuthenticationService, Application.Services.AuthenticationService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfiles>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

await SeedRolesAsync(app.Services);
await SeedOpenIddictAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/connect/token", async (HttpContext context) =>
{
    var request = context.GetOpenIddictServerRequest();
    if (request == null)
    {
        return Results.BadRequest();
    }

    if (request.IsPasswordGrantType())
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByNameAsync(request.Username);
        
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Results.BadRequest(new { error = "invalid_grant" });
        }

        var roles = await userManager.GetRolesAsync(user);
        
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, user.Id.ToString());
        identity.AddClaim(Claims.Name, user.UserName);
        identity.AddClaim(Claims.Email, user.Email);
        
        foreach (var role in roles)
        {
            identity.AddClaim(Claims.Role, role);
        }

        var principal = new ClaimsPrincipal(identity);
        return Results.SignIn(principal, new AuthenticationProperties(), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    return Results.BadRequest(new { error = "unsupported_grant_type" });
});

app.MapControllers();

app.Run();

static async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    
    string[] roles = { "Admin", "Donor", "CampaignManager" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}

static async Task SeedOpenIddictAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var clientId = configuration["OpenIddict:ClientId"] ?? "donation-client";
    var clientSecret = configuration["OpenIddict:ClientSecret"] ?? "donation-secret";
    var displayName = configuration["OpenIddict:DisplayName"] ?? "Donation Management System";

    if (await context.FindByClientIdAsync(clientId) == null)
    {
        await context.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            DisplayName = displayName,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles
            }
        });
    }
}
