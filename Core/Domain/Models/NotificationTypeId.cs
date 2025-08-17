namespace Domain.Models
{
    /// <summary>
    /// Enumerates supported notification template types.
    /// The numeric values are kept to allow seamless database storage as int.
    /// </summary>
    public enum NotificationTypeId
    {
        /// <summary>
        /// Email containing donation receipt (sent after successful donation).
        /// </summary>
        DonationReceipt = 1,

        /// <summary>
        /// Email sent when campaign reaches its goal (optional).
        /// </summary>
        CampaignGoalReached = 2,

        /// <summary>
        /// Email sent after successful user registration.
        /// </summary>
        Register = 3,

        /// <summary>
        /// Email sent to admin when payment gateway fails to create payment link.
        /// </summary>
        PaymentFailure = 4
    }
}
