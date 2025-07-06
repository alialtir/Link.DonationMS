using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs.DonationDTOs
{
    public class UpdateDonationStatusDto
    {
        [Required]
        public string Status { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }
    }
}
