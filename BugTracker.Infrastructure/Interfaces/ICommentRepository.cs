using BugTracker.Infrastructure.Models;

namespace BugTracker.Infrastructure.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<IEnumerable<Comment>> GetByBugIdAsync(Guid bugId);
    }

}
