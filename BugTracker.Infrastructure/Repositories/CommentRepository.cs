using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BugTracker.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BugTrackerDbContext _context;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(BugTrackerDbContext context, ILogger<CommentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Comment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving comment for bug ID {BugId}", comment.BugId);
                throw new Exception("Failed to add comment to database.", ex);
            }
        }

        public async Task<IEnumerable<Comment>> GetByBugIdAsync(Guid bugId)
        {
            try
            {
                return await _context.Comments
                    .Include(c => c.User)
                    .Where(c => c.BugId == bugId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving comments for bug ID {BugId}", bugId);
                throw new Exception("Failed to fetch comments for bug.", ex);
            }
        }
    }
}
