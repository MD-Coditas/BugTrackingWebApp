using BugTracker.Application.DTOs;
using BugTracker.Application.Enums;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Models.Filters;
using BugTracker.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "QA")]
    public class QAController : Controller
    {
        private readonly IBugService _bugService;
        private readonly ILogger<QAController> _logger;

        public QAController(IBugService bugService, ILogger<QAController> logger)
        {
            _bugService = bugService;
            _logger = logger;
        }

        [NoCache]
        public async Task<IActionResult> Dashboard(string keyword, string status, string priority, int page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>();
                ViewBag.Priorities = new[] { "Low", "Medium", "High" };
                ViewBag.PageSize = pageSize;

                var filter = new BugFilterDto
                {
                    Keyword = keyword,
                    Status = status,
                    Priority = priority
                };

                var pagedResult = await _bugService.GetFilteredBugsPagedAsync(filter, page, pageSize);

                var qaItems = pagedResult.Items
                    .Where(b => b.Status == "Resolved" || b.Status == "Closed")
                    .ToList();

                return View(new PagedResult<BugDto>
                {
                    Items = qaItems,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = qaItems.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading QA dashboard");
                Console.WriteLine($"[Dashboard Error]: {ex.Message}");
                ModelState.AddModelError("", "Could not load QA dashboard.");

                return View(new PagedResult<BugDto>
                {
                    Items = new List<BugDto>(),
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = 0
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            try
            {
                _logger.LogInformation("QA : {User} changed status of bug {BugId} to {NewStatus}", User.Identity?.Name, id, status);
                await _bugService.UpdateStatusAsync(id, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for bug ID {BugId}", id);
                Console.WriteLine($"[QA UpdateStatus Error]: {ex.Message}");
                TempData["Error"] = "Failed to update bug status.";
            }

            return RedirectToAction("Dashboard");
        }
    }

}
