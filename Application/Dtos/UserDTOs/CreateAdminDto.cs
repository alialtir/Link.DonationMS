using System.ComponentModel.DataAnnotations;

namespace DTOs.UserDTOs
{
    public class CreateAdminDto
    {
        [Required]
        public string DisplayName { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin or CampaignManager
    }
}
