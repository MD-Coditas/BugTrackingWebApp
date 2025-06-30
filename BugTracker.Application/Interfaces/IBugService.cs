using BugTracker.Application.DTOs;
using System;
using BugTracker.Infrastructure.Models.Filters;

namespace BugTracker.Application.Interfaces
{
    public interface IBugService
    {
        Task SubmitBugAsync(BugDto bugDto);
        Task<BugDto?> GetBugDetailsAsync(Guid bugId);
        Task UpdateStatusAsync(Guid bugId, string newStatus);
        Task<PagedResult<BugDto>> GetFilteredBugsPagedAsync(BugFilterDto filter, int page, int pageSize);
    }

}
