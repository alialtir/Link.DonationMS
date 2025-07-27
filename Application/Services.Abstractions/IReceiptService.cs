using DTOs.ReceiptDTOs;

namespace Application.Services.Abstractions
{
    public interface IReceiptService
    {
        Task<bool> GenerateReceiptAsync(int donationId);
        Task<ReceiptDto> GetByIdAsync(int id);
        Task<ReceiptDto> GetByDonationIdAsync(int donationId);
        Task<IEnumerable<ReceiptDto>> GetByUserAsync(Guid userId);
        Task<byte[]> GeneratePdfAsync(int receiptId);
        Task<bool> SendEmailAsync(int receiptId, string email);
    }
} 