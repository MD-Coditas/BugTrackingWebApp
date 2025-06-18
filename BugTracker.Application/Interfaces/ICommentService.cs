using BugTracker.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(CommentDto dto);
        Task<IEnumerable<CommentDto>> GetCommentsByBugIdAsync(Guid bugId);
    }

}
