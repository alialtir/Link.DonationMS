
using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using Persistence.Repositories;

namespace Link.DonationMS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<Persistence.Data.DonationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

                options.UseOpenIddict();
            });


            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DonationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddOpenIddict()
                .AddCore(options =>
                {
                    // Register the Entity Framework Core stores and models.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<DonationDbContext>();
                })
                .AddServer(options =>
                {
            
                    options.SetTokenEndpointUris("/connect/token");
                 
                    options.AllowAuthorizationCodeFlow();

                    options.AllowPasswordFlow();

                    options.AllowRefreshTokenFlow();

                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough();
        
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();



                });

            builder.Services.AddAuthentication();

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
