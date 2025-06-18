using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BugTrackerDbContext _context;

        public CommentRepository(BugTrackerDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Comment>> GetByBugIdAsync(Guid bugId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.BugId == bugId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }

}
