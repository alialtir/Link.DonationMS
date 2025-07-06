using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.EmailNotificationDTOs
{
    public class EmailNotificationDto
    {
        public int Id { get; set; }
        public int DonationId { get; set; }
        public string Type { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
