using DTOs.ReceiptDTOs;

namespace Application.Services.Abstractions
{
    public interface IReceiptService
    {
        Task<bool> GenerateReceiptAsync(Guid donationId);
        Task<ReceiptDto> GetByIdAsync(int id);
        Task<ReceiptDto> GetByDonationIdAsync(Guid donationId);
        Task<IEnumerable<ReceiptDto>> GetByUserAsync(Guid userId);
        Task<byte[]> GeneratePdfAsync(int receiptId);
        Task<bool> SendEmailAsync(int receiptId, string email);
    }
} 