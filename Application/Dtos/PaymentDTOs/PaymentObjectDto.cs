namespace DTOs.PaymentDTOs
{
    public class PaymentObjectDto
    {
        public string? PaymentUrl { get; set; }
        public string? PaymentId { get; set; }
        public PaymentObjectStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public enum PaymentObjectStatus
    {
        Success,
        Failed
    }
}
