using Application.Services.Abstractions;
using DTOs.ReceiptDTOs;

namespace Services
{
    public class ReceiptService : IReceiptService
    {
        public Task<ReceiptDto> CreateAsync(CreateReceiptDto createReceiptDto)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GeneratePdfAsync(int receiptId)
        {
            throw new NotImplementedException();
        }

        public Task<ReceiptDto> GetByDonationIdAsync(int donationId)
        {
            throw new NotImplementedException();
        }

        public Task<ReceiptDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReceiptDto>> GetByUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmailAsync(int receiptId, string email)
        {
            throw new NotImplementedException();
        }
    }
} 