using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DTOs.NotificationDTOs
{
    public class CreateNotificationDto
    {
        public string? To { get; set; }
        public string? CC { get; set; }
        public string? BCC { get; set; }
        
        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;
        
        public int LanguageId { get; set; } = 1; // Default to Arabic
        
        [Required]
        public int TypeId { get; set; }
        
        public string? Parameters { get; set; }
        
        public Guid? UserId { get; set; }
    }
}
