using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace BugTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepo, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<bool> RegisterAsync(UserDto userDto)
        {
            try
            {
                var userExists = await _userRepo.GetByEmailAsync(userDto.Email!);
                if (userExists != null) return false;

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = userDto.UserName!,
                    Email = userDto.Email!,
                    PasswordHash = HashPassword(userDto.Password!),
                    Role = userDto.Role,
                    CreatedAt = DateTime.Now
                };

                await _userRepo.CreateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with email: {Email}", userDto.Email);
                throw new ApplicationException("User registration failed.", ex);
            }

        }

        public async Task<UserDto?> ValidateUserAsync(string email, string password)
        {
            try
            {
                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null || !VerifyPassword(password, user.PasswordHash)) return null;

                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials for email: {Email}", email);
                throw new ApplicationException("Failed to validate user credentials.", ex);
            }

        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return !string.IsNullOrEmpty(hash) && BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepo.GetAllAsync();
                return users.Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw new ApplicationException("Failed to fetch users.", ex);
            }

        }

        public async Task UpdateUserRoleAsync(Guid userId, string newRole)
        {
            try
            {
                await _userRepo.UpdateRoleAsync(userId, newRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for user ID {UserId}", userId);
                throw new ApplicationException("Failed to update user role.", ex);
            }

        }

        public async Task<PagedResult<UserDto>> GetFilteredUsersPagedAsync(string? search, int page, int pageSize)
        {
            try
            {
                var all = (await GetAllUsersAsync())
                    .Where(u => (u.Role == "User" || u.Role == "QA") &&
                                (string.IsNullOrEmpty(search) ||
                                 u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                 u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new PagedResult<UserDto>
                {
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = all.Count,
                    Items = paged
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering users with search string: {Search}", search);
                throw new ApplicationException("Failed to filter users.", ex);
            }

        }
    }
}
