using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using BugTracker.Infrastructure.Models.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace BugTracker.Application.Services
{
    public class BugService : IBugService
    {
        private readonly IBugRepository _bugRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;
        private readonly ILogger<BugService> _logger;

        public BugService(IBugRepository bugRepository, IWebHostEnvironment env, IEmailService emailService, ILogger<BugService> logger)
        {
            _bugRepository = bugRepository;
            _env = env;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SubmitBugAsync(BugDto dto)
        {
            try
            {
                string? screenshotPath = null;

                if (dto.Screenshot != null)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);
                    var fileName = Guid.NewGuid() + Path.GetExtension(dto.Screenshot.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.Screenshot.CopyToAsync(stream);
                    }

                    screenshotPath = "/uploads/" + fileName;
                }

                var bug = new Bug
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title!,
                    Description = dto.Description!,
                    Priority = dto.Priority!,
                    ScreenshotPath = screenshotPath,
                    CreatedAt = DateTime.Now,
                    ReporterId = dto.ReporterId,
                    Status = "Open"
                };

                await _bugRepository.AddAsync(bug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting bug for user {UserId}", dto.ReporterId);
                throw new ApplicationException("Failed to submit the bug.", ex);
            }

        }

        public async Task<BugDto?> GetBugDetailsAsync(Guid bugId)
        {
            try
            {
                var bug = await _bugRepository.GetByIdAsync(bugId);
                if (bug == null) return null;

                return new BugDto
                {
                    Id = bug.Id,
                    Title = bug.Title,
                    Description = bug.Description,
                    Priority = bug.Priority,
                    Status = bug.Status,
                    ScreenshotPath = bug.ScreenshotPath,
                    CreatedAt = bug.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bug details for ID {BugId}", bugId);
                throw new ApplicationException("Failed to retrieve bug details.", ex);
            }

        }

        public async Task UpdateStatusAsync(Guid bugId, string newStatus)
        {
            try
            {
                var bug = await _bugRepository.GetByIdAsync(bugId);
                if (bug == null) return;

                var oldStatus = bug.Status;

                if (oldStatus != newStatus)
                {
                    bug.Status = newStatus;
                    await _bugRepository.UpdateStatusAsync(bugId, newStatus);

                    var emailBody = $@"
                        <h3>Bug Status Updated</h3>
                        <p><strong>Title:</strong> {bug.Title}</p>
                        <p><strong>Old Status:</strong> {oldStatus}</p>
                        <p><strong>New Status:</strong> {newStatus}</p>
                        <p><strong>Updated At:</strong> {DateTime.Now}</p>";

                    await _emailService.SendEmailAsync(bug.Reporter.Email, "Bug Status Updated", emailBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for bug ID {BugId}", bugId);
                throw new ApplicationException("Failed to update bug status.", ex);
            }

        }

        public async Task<PagedResult<BugDto>> GetFilteredBugsPagedAsync(BugFilterDto filter, int page, int pageSize)
        {
            try
            {
                var all = await _bugRepository.GetFilteredAsync(filter);
                var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new PagedResult<BugDto>
                {
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = all.Count(),
                    Items = items.Select(b => new BugDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Description = b.Description,
                        Priority = b.Priority,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt,
                        ScreenshotPath = b.ScreenshotPath,
                        ReporterId = b.ReporterId
                    })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching filtered bug list with filter: {@Filter}", filter);
                throw new ApplicationException("Failed to fetch filtered bugs.", ex);
            }

        }
    }
}
