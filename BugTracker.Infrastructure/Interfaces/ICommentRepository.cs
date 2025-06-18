using BugTracker.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Infrastructure.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<IEnumerable<Comment>> GetByBugIdAsync(Guid bugId);
    }

}
