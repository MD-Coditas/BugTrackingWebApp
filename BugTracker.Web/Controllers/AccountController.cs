using BugTracker.Application.Interfaces;
using BugTracker.Application.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BugTracker.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;

namespace BugTracker.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IValidator<UserDto> _userValidator;

        public AccountController(IUserService userService, IValidator<UserDto> userValidator)
        {
            _userService = userService;
            _userValidator = userValidator;
        }


        [HttpGet, NoCache]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return role switch
                {
                    "Admin" => RedirectToAction("AllBugs", "Admin"),
                    "QA" => RedirectToAction("Dashboard", "QA"),
                    _ => RedirectToAction("Report", "Bug")
                };
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserDto dto)
        {
            var user = await _userService.ValidateUserAsync(dto.Email, dto.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(dto);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);
            switch (user.Role)
            {
                case "Admin":
                    return RedirectToAction("AllBugs", "Admin");

                case "QA":
                    return RedirectToAction("Dashboard", "QA");

                case "User":
                default:
                    return RedirectToAction("Report", "Bug");
            }
        }

        [HttpGet, NoCache]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return role switch
                {
                    "Admin" => RedirectToAction("AllBugs", "Admin"),
                    "QA" => RedirectToAction("Dashboard", "QA"),
                    _ => RedirectToAction("Report", "Bug")
                };
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserDto dto)
        {
            var result = await _userValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(dto);
            }

            dto.Role = "User";
            var success = await _userService.RegisterAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(dto);
            }

            return RedirectToAction("Login");
        }


        [NoCache]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }
    }

}
