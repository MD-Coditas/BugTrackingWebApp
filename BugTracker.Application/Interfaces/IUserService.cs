using BugTracker.Application.DTOs;

namespace BugTracker.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(UserDto userDto);
        Task<UserDto?> ValidateUserAsync(string email, string password);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task UpdateUserRoleAsync(Guid userId, string newRole);
        Task<PagedResult<UserDto>> GetFilteredUsersPagedAsync(string? search, int page, int pageSize);
    }
}
