namespace DTOs.DonationDTOs
{
    public class DonationPaymentResultDto
    {
        public Guid DonationId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentUrl { get; set; }
        public string? PaymentId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
