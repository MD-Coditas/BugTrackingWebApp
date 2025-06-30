using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BugTracker.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BugTrackerDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(BugTrackerDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email: {Email}", email);
                throw new Exception("Failed to retrieve user by email.", ex);
            }
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new user: {Email}", user.Email);
                throw new Exception("Failed to create new user.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                throw new Exception("Failed to retrieve all users.", ex);
            }
        }

        public async Task UpdateRoleAsync(Guid userId, string newRole)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Role = newRole;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for user ID {UserId}", userId);
                throw new Exception("Failed to update user role.", ex);
            }
        }
    }
}
