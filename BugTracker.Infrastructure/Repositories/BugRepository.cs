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
    public class BugRepository : IBugRepository
    {
        private readonly BugTrackerDbContext _context;

        public BugRepository(BugTrackerDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Bug bug)
        {
            _context.Bugs.Add(bug);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Bug>> GetByReporterIdAsync(Guid reporterId)
        {
            return await _context.Bugs
                .Where(b => b.ReporterId == reporterId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bug?> GetByIdAsync(Guid id)
        {
            return await _context.Bugs
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }

}
