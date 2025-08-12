using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.UserDTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        
        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string DisplayNameAr { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }
    }
}
