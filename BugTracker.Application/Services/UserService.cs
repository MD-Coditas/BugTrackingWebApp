using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using BugTracker.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<bool> RegisterAsync(UserDto userDto)
        {
            var userExists = await _userRepo.GetByEmailAsync(userDto.Email);
            if (userExists != null) return false;

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userDto.UserName,
                Email = userDto.Email,
                PasswordHash = HashPassword(userDto.Password),
                Role = userDto.Role,
                CreatedAt = DateTime.Now
            };


            await _userRepo.CreateAsync(user);
            return true;
        }

        public async Task<UserDto?> ValidateUserAsync(string email, string password)
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

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            return user == null ? null : new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = user.Role
            };
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
            var users = await _userRepo.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,  
                Role = u.Role
            });
        }


        public async Task UpdateUserRoleAsync(Guid userId, string newRole)
        {
            await _userRepo.UpdateRoleAsync(userId, newRole);
        }

    }

}
