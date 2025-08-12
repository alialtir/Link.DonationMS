using System.Threading.Tasks;

namespace Application.Services.Abstractions
{
    /// <summary>
    /// Base interface for email service providers
    /// </summary>
    public interface IEmailGatewayService
    {
        /// <summary>
        /// Send an email
        /// </summary>
        Task<bool> SendEmailAsync(string to, string subject, string body);

        /// <summary>
        /// Test the connection to the email server
        /// </summary>
        Task<bool> TestConnectionAsync();
    }
}
