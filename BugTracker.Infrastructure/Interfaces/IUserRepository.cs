﻿using BugTracker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task UpdateRoleAsync(Guid userId, string newRole);
    }
}
