using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using BugTracker.Infrastructure.Models.Filters;
using BugTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.Services
{
    public class BugService : IBugService
    {
        private readonly IBugRepository _bugRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public BugService(IBugRepository bugRepository, IWebHostEnvironment env, IEmailService emailService)
        {
            _bugRepository = bugRepository;
            _env = env;
            _emailService = emailService;
        }

        public async Task SubmitBugAsync(BugDto dto)
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

        public async Task<BugDto?> GetBugDetailsAsync(Guid bugId)
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

        public async Task UpdateStatusAsync(Guid bugId, string newStatus)
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
        
        public async Task<PagedResult<BugDto>> GetFilteredBugsPagedAsync(BugFilterDto filter, int page, int pageSize)
        {
            var all = await _bugRepository.GetFilteredAsync(filter);
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResult<BugDto>
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

            return result;
        }
    }
}