using BugTracker.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(UserDto userDto);
        Task<UserDto?> ValidateUserAsync(string email, string password);
        Task<UserDto?> GetByEmailAsync(string email);
    }
}
