using BugTracker.Application.DTOs;

namespace BugTracker.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(UserDto userDto);
        Task<UserDto?> ValidateUserAsync(string email, string password);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task UpdateUserRoleAsync(Guid userId, string newRole);

    }
}
