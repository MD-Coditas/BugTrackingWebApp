using BugTracker.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.Interfaces
{
    public interface IBugService
    {
        Task SubmitBugAsync(BugDto bugDto);
        Task<IEnumerable<BugDto>> GetUserBugsAsync(Guid userId);
        Task<BugDto?> GetBugDetailsAsync(Guid bugId);
    }

}
