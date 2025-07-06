using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ReceiptDTOs
{
    public class ReceiptDto
    {
        public int Id { get; set; }
        public int DonationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
