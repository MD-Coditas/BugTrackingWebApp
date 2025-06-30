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

        public QAController(IBugService bugService)
        {
            _bugService = bugService;
        }

        [NoCache]
        [Authorize(Roles = "QA")]
        public async Task<IActionResult> Dashboard(string keyword, string status, string priority, int page = 1, int pageSize = 10)
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



        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            await _bugService.UpdateStatusAsync(id, status);
            return RedirectToAction("Dashboard");
        }
    }
}
