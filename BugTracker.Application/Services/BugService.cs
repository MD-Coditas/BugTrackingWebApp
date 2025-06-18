using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
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

        public BugService(IBugRepository bugRepository, IWebHostEnvironment env)
        {
            _bugRepository = bugRepository;
            _env = env;
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
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                ScreenshotPath = screenshotPath,
                CreatedAt = DateTime.Now,
                ReporterId = dto.ReporterId,
                Status = "Open"
            };

            await _bugRepository.AddAsync(bug);
        }

        public async Task<IEnumerable<BugDto>> GetUserBugsAsync(Guid userId)
        {
            var bugs = await _bugRepository.GetByReporterIdAsync(userId);

            return bugs.Select(b => new BugDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Priority = b.Priority,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                ScreenshotPath = b.ScreenshotPath
            });
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

    }

}
