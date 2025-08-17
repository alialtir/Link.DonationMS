using DTOs.ReceiptDTOs;

namespace Application.Services.Abstractions
{
    /// <summary>
    /// Gateway interface for PDF generation providers
    /// Similar to IPaymentGatewayService pattern
    /// </summary>
    public interface IPdfGatewayService
    {
        /// <summary>
        /// Generate PDF document as byte array
        /// </summary>
        /// <param name="receiptData">Receipt data for PDF generation</param>
        /// <returns>PDF document as byte array</returns>
        Task<byte[]> GeneratePdfAsync(PdfReceiptDataDto receiptData);
    }
}
