using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Web.Filters;
using BugTracker.Application.Interfaces;
using BugTracker.Application.Enums;
using BugTracker.Infrastructure.Models.Filters;
using BugTracker.Application.DTOs;
using BugTracker.Infrastructure.Models;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBugService _bugService;
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IBugService bugService, IUserService userService, ILogger<AdminController> logger)
        {
            _bugService = bugService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> AllBugs(string keyword, string status, Guid? reporterId, int page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>();
                ViewBag.Reporters = await _userService.GetAllUsersAsync();
                ViewBag.PageSize = pageSize;

                var filter = new BugFilterDto
                {
                    Keyword = keyword,
                    Status = status,
                    ReporterId = reporterId
                };

                var result = await _bugService.GetFilteredBugsPagedAsync(filter, page, pageSize);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading AllBugs view");
                Console.WriteLine($"[AllBugs Error]: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            try
            {
                _logger.LogInformation("Admin updated status of bug (ID: {BugId}) to '{New}'", id, status);
                await _bugService.UpdateStatusAsync(id, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for bug ID {BugId}", id);
                Console.WriteLine($"[UpdateStatus Error]: {ex.Message}");
                TempData["Error"] = "Failed to update bug status.";
            }

            return RedirectToAction("AllBugs");
        }

        public async Task<IActionResult> ManageRoles(string search, int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _userService.GetFilteredUsersPagedAsync(search, page, pageSize);
                ViewBag.Search = search;
                ViewBag.PageSize = pageSize;
                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ManageRoles Error]: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoles(List<UserDto> users)
        {
            try
            {
                foreach (var user in users)
                {
                    _logger.LogInformation("Admin changed role of user {UserId} to {NewRole}", user.Id, user.Role);
                    await _userService.UpdateUserRoleAsync(user.Id, user.Role);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateRoles Error]: {ex.Message}");
                TempData["Error"] = "Failed to update roles.";
            }

            return RedirectToAction("ManageRoles");
        }
    }


}
