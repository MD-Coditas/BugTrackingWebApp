using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Web.Filters;
using System.Security.Claims;
using FluentValidation;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "User,Admin,QA")]
    public class BugController : Controller
    {
        private readonly IBugService _bugService;
        private readonly IValidator<BugDto> _bugValidator;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICommentService _commentService;

        public BugController(IBugService bugService, IValidator<BugDto> bugValidator, IHttpContextAccessor httpContext, ICommentService commentService)
        {
            _bugService = bugService;
            _bugValidator = bugValidator;
            _httpContext = httpContext;
            _commentService = commentService;
        }

        private Guid GetUserId()
        {
            var claim = _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

        [NoCache]
        public IActionResult Report()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Report(BugDto dto)
        {
            var validationResult = await _bugValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(dto);
            }

            dto.ReporterId = GetUserId();
            await _bugService.SubmitBugAsync(dto);
            return RedirectToAction("MyBugs");
        }


        [NoCache]
        public async Task<IActionResult> MyBugs()
        {
            var userId = GetUserId();
            var bugs = await _bugService.GetUserBugsAsync(userId);
            return View(bugs);
        }

        [NoCache]
        public async Task<IActionResult> Details(Guid id)
        {
            var bug = await _bugService.GetBugDetailsAsync(id);
            if (bug == null) return NotFound();

            var comments = await _commentService.GetCommentsByBugIdAsync(id);

            ViewBag.Comments = comments;
            ViewBag.BugId = id;
            return View(bug);
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(Guid bugId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Details", new { id = bugId });

            var comment = new CommentDto
            {
                BugId = bugId,
                UserId = GetUserId(),
                Message = message
            };

            await _commentService.AddCommentAsync(comment);

            return RedirectToAction("Details", new { id = bugId });
        }


    }

}
