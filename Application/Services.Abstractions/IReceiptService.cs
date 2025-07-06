using DTOs.ReceiptDTOs;

namespace Application.Services.Abstractions
{
    public interface IReceiptService
    {
        Task<ReceiptDto> GetByIdAsync(int id);
        Task<ReceiptDto> GetByDonationIdAsync(int donationId);
        Task<IEnumerable<ReceiptDto>> GetByUserAsync(Guid userId);
        Task<ReceiptDto> CreateAsync(CreateReceiptDto createReceiptDto);
        Task<byte[]> GeneratePdfAsync(int receiptId);
        Task<bool> SendEmailAsync(int receiptId, string email);
    }
} 