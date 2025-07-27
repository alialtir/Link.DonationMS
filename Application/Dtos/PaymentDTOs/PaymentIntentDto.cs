namespace DTOs.PaymentDTOs
{
    public class PaymentIntentDto
    {
        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public int DonationId { get; set; }
    }
} 