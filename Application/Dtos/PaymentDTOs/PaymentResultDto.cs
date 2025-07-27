namespace DTOs.PaymentDTOs
{
    public class PaymentResultDto
    {
        public bool Succeeded { get; set; }
        public string PaymentIntentId { get; set; }
        public string ErrorMessage { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime ProcessedAt { get; set; }
    }
} 