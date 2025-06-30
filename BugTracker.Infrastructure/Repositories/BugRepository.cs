using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using BugTracker.Infrastructure.Models.Filters;
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
                .Include(b => b.Reporter)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<IEnumerable<Bug>> GetAllAsync()
        {
            return await _context.Bugs
                .Include(b => b.Reporter)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
        public async Task UpdateStatusAsync(Guid bugId, string newStatus)
        {
            var bug = await _context.Bugs.FindAsync(bugId);
            if (bug == null) return;

            bug.Status = newStatus;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Bug>> GetFilteredAsync(BugFilterDto filter)
        {
            var query = _context.Bugs.Include(b => b.Reporter).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                string keyword = filter.Keyword.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(keyword) ||
                    b.Description.ToLower().Contains(keyword) ||
                    b.Reporter.UserName.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(b => b.Status == filter.Status);
            }
            if (!string.IsNullOrWhiteSpace(filter.Priority))
            {
                query = query.Where(b => b.Priority == filter.Priority);
            }


            if (filter.ReporterId.HasValue)
            {
                query = query.Where(b => b.ReporterId == filter.ReporterId);
            }

            return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        }


    }

}
