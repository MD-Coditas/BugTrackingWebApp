using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Web.Filters;
using System.Security.Claims;
using FluentValidation;
using BugTracker.Application.Enums;
using BugTracker.Infrastructure.Models.Filters;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "User,Admin,QA")]
    public class BugController : Controller
    {
        private readonly IBugService _bugService;
        private readonly IValidator<BugDto> _bugValidator;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICommentService _commentService;
        private readonly ILogger<BugController> _logger;

        public BugController(IBugService bugService, IValidator<BugDto> bugValidator, IHttpContextAccessor httpContext, ICommentService commentService, ILogger<BugController> logger)
        {
            _bugService = bugService;
            _bugValidator = bugValidator;
            _httpContext = httpContext;
            _commentService = commentService;
            _logger = logger;
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
            try
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
                _logger.LogInformation("User {UserId} reported a new bug titled '{Title}'", dto.ReporterId, dto.Title);
                return RedirectToAction("MyBugs");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting bug for user {UserId}", GetUserId());
                Console.WriteLine($"[Report Bug Error]: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while submitting the bug.");
                return View(dto);
            }
        }

        [Authorize(Roles = "User")]
        [NoCache]
        public async Task<IActionResult> MyBugs(string keyword, string status, string priority, int page = 1, int pageSize = 10)
        {
            try
            {
                ViewBag.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>();
                ViewBag.Priorities = new[] { "Low", "Medium", "High" };
                ViewBag.PageSize = pageSize;

                var userId = GetUserId();
                var filter = new BugFilterDto
                {
                    Keyword = keyword,
                    Status = status,
                    Priority = priority,
                    ReporterId = userId
                };

                var result = await _bugService.GetFilteredBugsPagedAsync(filter, page, pageSize);
                return View(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyBugs Error]: {ex.Message}");
                ModelState.AddModelError("", "Could not load your reported bugs.");
                return View(new PagedResult<BugDto>
                {
                    Items = new List<BugDto>(),
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = 0
                });
            }
        }

        [NoCache]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var bug = await _bugService.GetBugDetailsAsync(id);
                if (bug == null) return NotFound();

                var comments = await _commentService.GetCommentsByBugIdAsync(id);

                ViewBag.Comments = comments;
                ViewBag.BugId = id;
                return View(bug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bug details for ID {BugId}", id);
                Console.WriteLine($"[Bug Details Error]: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(Guid bugId, string message)
        {
            try
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bug details for ID {BugId}", bugId);
                Console.WriteLine($"[PostComment Error]: {ex.Message}");
                TempData["Error"] = "Could not add your comment.";
            }

            return RedirectToAction("Details", new { id = bugId });
        }
    }


}
