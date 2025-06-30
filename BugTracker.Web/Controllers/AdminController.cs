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

        public async Task<IActionResult> AllBugs(string keyword, string status, Guid? reporterId, int page = 1, int pageSize = 10)
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


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            await _bugService.UpdateStatusAsync(id, status);
            return RedirectToAction("AllBugs");
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRoles(string search, int page = 1, int pageSize = 10)
        {
            var result = await _userService.GetFilteredUsersPagedAsync(search, page, pageSize);
            ViewBag.Search = search;
            ViewBag.PageSize = pageSize;
            return View(result);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoles(List<UserDto> users)
        {
            foreach (var user in users)
            {
                await _userService.UpdateUserRoleAsync(user.Id, user.Role);
            }
            return RedirectToAction("ManageRoles");
        }
    }

}
