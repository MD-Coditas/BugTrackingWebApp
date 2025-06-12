using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "User,Admin,QA")]
    public class BugController : Controller
    {
        private readonly IBugService _bugService;
        private readonly IHttpContextAccessor _httpContext;

        public BugController(IBugService bugService, IHttpContextAccessor httpContext)
        {
            _bugService = bugService;
            _httpContext = httpContext;
        }

        private Guid GetUserId()
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

        public IActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Report(BugDto dto)
        {
            dto.ReporterId = GetUserId();
            await _bugService.SubmitBugAsync(dto);
            return RedirectToAction("MyBugs");
        }

        public async Task<IActionResult> MyBugs()
        {
            var userId = GetUserId();
            var bugs = await _bugService.GetUserBugsAsync(userId);
            return View(bugs);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var bug = await _bugService.GetBugDetailsAsync(id);
            if (bug == null) return NotFound();

            return View(bug);
        }
    }

}
