using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ReceiptDTOs
{
    public class CreateReceiptDto
    {
        [Required]
        public int DonationId { get; set; }
    }
}
