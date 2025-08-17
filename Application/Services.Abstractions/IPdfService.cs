namespace Application.Services.Abstractions
{
    /// <summary>
    /// High-level PDF service interface
    /// Handles business logic and delegates PDF generation to gateway
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Generate receipt PDF by receipt ID
        /// </summary>
        /// <param name="receiptId">Receipt ID</param>
        /// <returns>PDF as byte array</returns>
        Task<byte[]> GenerateReceiptPdfAsync(int receiptId);
    }
}
