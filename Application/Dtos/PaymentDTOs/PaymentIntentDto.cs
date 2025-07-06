namespace DTOs.PaymentDTOs
{
    public class PaymentIntentDto
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string ClientSecret { get; set; }
        public int DonationId { get; set; }
    }
} 