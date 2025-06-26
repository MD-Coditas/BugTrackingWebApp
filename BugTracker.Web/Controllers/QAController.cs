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
        public async Task<IActionResult> Dashboard(string keyword, string status, string priority)
        {
            ViewBag.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>();
            ViewBag.Priorities = new[] { "Low", "Medium", "High" };

            var filter = new BugFilterDto
            {
                Keyword = keyword,
                Status = status,
                Priority = priority
            };

            var bugs = await _bugService.GetFilteredBugsAsync(filter);
            var qaBugs = bugs.Where(b => b.Status == "Resolved" || b.Status == "Closed").ToList();

            return View(qaBugs);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            await _bugService.UpdateStatusAsync(id, status);
            return RedirectToAction("Dashboard");
        }
    }
}
