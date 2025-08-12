using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.EntityFrameworkCore;

namespace Persistence.Data
{
    public class DonationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DonationDbContext(DbContextOptions<DonationDbContext> options) : base(options)
        {
        }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Donation> Donations { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Receipt> Receipts { get; set; }


        // Added for notification system
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyRef).Assembly);

            // Explicitly configure Notification entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.To).IsRequired(false);
                entity.Property(e => e.CC).IsRequired(false);
                entity.Property(e => e.BCC).IsRequired(false);
                entity.Property(e => e.UserId).IsRequired(false);

                // Ensure no DonationId column is created
                entity.Ignore("DonationId");

                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                //entity.HasOne(n => n.NotificationType)
                //    .WithMany()
                //    .HasForeignKey(n => n.NotificationTypeId)
                //    .OnDelete(DeleteBehavior.Restrict);
            });

            // ✅ Seeding NotificationTypes
            modelBuilder.Entity<NotificationType>().HasData(
                new NotificationType
                {
                    Id = 1,
                    TypeId = NotificationTypeId.DonationReceipt,
                    Subject = "شكراً لتبرعك يا {{DonorName}}",
                    Body = "تم استلام تبرعك بمبلغ {{Amount}} لحملة {{CampaignName}}. رقم الإيصال: {{ReceiptNumber}}",
                    LanguageId = NotificationLanguage.Arabic
                },
                new NotificationType
                {
                    Id = 2,
                    TypeId = NotificationTypeId.DonationReceipt,
                    Subject = "Thank you for your donation {{DonorName}}",
                    Body = "We have received your donation of {{Amount}} to {{CampaignName}}. Receipt No: {{ReceiptNumber}}",
                    LanguageId = NotificationLanguage.English
                },
                new NotificationType
                {
                    Id = 3,
                    TypeId = NotificationTypeId.Register,
                    Subject = "مرحباً بك في منصة التبرعات",
                    Body = "مرحباً {{UserName}},\n\nشكراً لتسجيلك في منصة التبرعات. يمكنك الآن تسجيل الدخول والبدء في دعم الحملات المفضلة لديك.",
                    LanguageId = NotificationLanguage.Arabic
                },
                new NotificationType
                {
                    Id = 4,
                    TypeId = NotificationTypeId.Register,
                    Subject = "Welcome to Donation Platform",
                    Body = "Hello {{UserName}},\n\nThank you for registering with our donation platform. You can now log in and start supporting your favorite campaigns.",
                    LanguageId = NotificationLanguage.English
                },
                new NotificationType
                {
                    Id = 5,
                    TypeId = NotificationTypeId.CampaignGoalReached,
                    Subject = "تم تحقيق هدف حملة {{CampaignName}}",
                    Body = "نود إبلاغك بأنه تم تحقيق هدف حملة {{CampaignName}}. شكراً لدعمكم الكريم",
                    LanguageId = NotificationLanguage.Arabic
                },
                new NotificationType
                {
                    Id = 6,
                    TypeId = NotificationTypeId.CampaignGoalReached,
                    Subject = "Campaign {{CampaignName}} Goal Reached",
                    Body = "We are pleased to inform you that the campaign {{CampaignName}} has reached its funding goal. Thank you for your support!",
                    LanguageId = NotificationLanguage.English
                }
            );

            base.OnModelCreating(modelBuilder);
            modelBuilder.UseOpenIddict<Guid>();
        }



    }
}
