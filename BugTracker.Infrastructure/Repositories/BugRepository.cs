using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using BugTracker.Infrastructure.Models.Filters;
using Microsoft.Extensions.Logging;

namespace BugTracker.Infrastructure.Repositories
{
    public class BugRepository : IBugRepository
    {
        private readonly BugTrackerDbContext _context;
        private readonly ILogger<BugRepository> _logger;

        public BugRepository(BugTrackerDbContext context, ILogger<BugRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Bug bug)
        {
            try
            {
                _context.Bugs.Add(bug);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bug to database");
                throw new Exception("Failed to add bug to database.", ex);
            }
        }

        public async Task<IEnumerable<Bug>> GetByReporterIdAsync(Guid reporterId)
        {
            try
            {
                return await _context.Bugs
                    .Where(b => b.ReporterId == reporterId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bugs for reporter ID {ReporterId}", reporterId);
                throw new Exception("Failed to retrieve bugs by reporter.", ex);
            }
        }

        public async Task<Bug?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Bugs
                    .Include(b => b.Comments)
                    .Include(b => b.Reporter)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bug by ID {BugId}", id);
                throw new Exception("Failed to fetch bug by ID.", ex);
            }
        }

        public async Task<IEnumerable<Bug>> GetAllAsync()
        {
            try
            {
                return await _context.Bugs
                    .Include(b => b.Reporter)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bugs");
                throw new Exception("Failed to retrieve all bugs.", ex);
            }
        }

        public async Task UpdateStatusAsync(Guid bugId, string newStatus)
        {
            try
            {
                var bug = await _context.Bugs.FindAsync(bugId);
                if (bug == null) return;

                bug.Status = newStatus;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bug status for bug ID {BugId}", bugId);
                throw new Exception("Failed to update bug status.", ex);
            }
        }

        public async Task<IEnumerable<Bug>> GetFilteredAsync(BugFilterDto filter)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering bugs with filter: {@Filter}", filter);
                throw new Exception("Failed to filter bugs.", ex);
            }
        }
    }
}
