using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Web.Filters;
using BugTracker.Application.Interfaces;
using BugTracker.Application.Enums;
using BugTracker.Infrastructure.Models.Filters;
using BugTracker.Application.DTOs;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBugService _bugService;
        private readonly IUserService _userService;

        public AdminController(IBugService bugService, IUserService userService)
        {
            _bugService = bugService;
            _userService = userService;
        }

        public async Task<IActionResult> AllBugs(string keyword, string status, Guid? reporterId)
        {
            ViewBag.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>();
            ViewBag.Reporters = await _userService.GetAllUsersAsync();

            var filter = new BugFilterDto
            {
                Keyword = keyword,
                Status = status,
                ReporterId = reporterId
            };

            var bugs = await _bugService.GetFilteredBugsAsync(filter);
            return View(bugs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            await _bugService.UpdateStatusAsync(id, status);
            return RedirectToAction("AllBugs");
        }

    }

}
