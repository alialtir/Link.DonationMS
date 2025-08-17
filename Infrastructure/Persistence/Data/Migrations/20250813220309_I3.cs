using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class I3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NotificationTypes",
                columns: new[] { "Id", "Body", "LanguageId", "Subject", "TypeId" },
                values: new object[,]
                {
                    { 7, "تم فشل في إنشاء رابط الدفع للتبرع التالي:\n\nرقم التبرع: {{DonationId}}\nالمبلغ: {{Amount}} ريال\nرقم الحملة: {{CampaignId}}\nرقم المستخدم: {{UserId}}\nتاريخ التبرع: {{DonationDate}}\nسبب الفشل: {{PaymentError}}\nبوابة الدفع: {{PaymentGateway}}\n\nيرجى التحقق من إعدادات بوابة الدفع.", 1, "فشل في إنشاء رابط الدفع - تبرع رقم {{DonationId}}", 4 },
                    { 8, "Payment link creation failed for the following donation:\n\nDonation ID: {{DonationId}}\nAmount: {{Amount}} SAR\nCampaign ID: {{CampaignId}}\nUser ID: {{UserId}}\nDonation Date: {{DonationDate}}\nError: {{PaymentError}}\nPayment Gateway: {{PaymentGateway}}\n\nPlease check payment gateway settings.", 2, "Payment Link Creation Failed - Donation {{DonationId}}", 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
