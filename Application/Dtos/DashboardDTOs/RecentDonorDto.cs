using System;

namespace DTOs.DashboardDTOs
{
    /// <summary>
    /// Represents a donor entry for recent donors widget.
    /// If the donation was anonymous, Name will be "Anonymous".
    /// </summary>
    public class RecentDonorDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
