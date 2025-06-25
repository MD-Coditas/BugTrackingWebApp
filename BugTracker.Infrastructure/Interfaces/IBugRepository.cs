using BugTracker.Infrastructure.Models;
using BugTracker.Infrastructure.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Infrastructure.Interfaces
{
    public interface IBugRepository
    {
        Task AddAsync(Bug bug);
        Task<IEnumerable<Bug>> GetByReporterIdAsync(Guid reporterId);
        Task<Bug?> GetByIdAsync(Guid id);
        Task<IEnumerable<Bug>> GetAllAsync();
        Task UpdateStatusAsync(Guid bugId, string newStatus);
        Task<IEnumerable<Bug>> GetFilteredAsync(BugFilterDto filter);
    }

}
