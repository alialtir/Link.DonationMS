namespace Application.Dtos.UserDTOs
{
    public class CreateGoogleUserDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; } = "Admin";
    }
}
