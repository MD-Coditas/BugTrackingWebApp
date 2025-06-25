using BugTracker.Application.DTOs;
using System;
using BugTracker.Infrastructure.Models.Filters;

namespace BugTracker.Application.Interfaces
{
    public interface IBugService
    {
        Task SubmitBugAsync(BugDto bugDto);
        Task<IEnumerable<BugDto>> GetUserBugsAsync(Guid userId);
        Task<BugDto?> GetBugDetailsAsync(Guid bugId);
        Task<IEnumerable<BugDto>> GetAllBugsAsync();
        Task UpdateStatusAsync(Guid bugId, string newStatus);
        Task<IEnumerable<BugDto>> GetFilteredBugsAsync(BugFilterDto filter);
    }

}
