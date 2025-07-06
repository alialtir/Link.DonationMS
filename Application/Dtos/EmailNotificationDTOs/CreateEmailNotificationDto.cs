using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.EmailNotificationDTOs
{
    public class CreateEmailNotificationDto
    {
        [Required]
        public int DonationId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required, EmailAddress]
        public string RecipientEmail { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
