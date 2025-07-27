using DTOs.UserDTOs;

namespace Application.Services.Abstractions
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> GetByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> RegisterAsync(RegisterUserDto registerUserDto);
        Task<UserDto> UpdateAsync(Guid id, UserDto userDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ChangeUserRoleAsync(Guid id, string role);
        
        Task<bool> IsInRoleAsync(Guid userId, string role);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
        Task<bool> AddToRoleAsync(Guid userId, string role);
        Task<bool> RemoveFromRoleAsync(Guid userId, string role);
        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string role);
        Task<bool> ResetPasswordByEmailAsync(string email, string newPassword);
    }
} 