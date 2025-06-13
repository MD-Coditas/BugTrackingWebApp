using BugTracker.Application.Interfaces;
using BugTracker.Application.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BugTracker.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Login() => View();

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
                    return RedirectToAction("Dashboard", "Admin");

                case "QA":
                    return RedirectToAction("Dashboard", "QA");

                case "User":
                default:
                    return RedirectToAction("Report", "Bug"); // Or any user-specific page
            }
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            dto.Role = "User";

            var result = await _userService.RegisterAsync(dto);
            if (!result)
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(dto);
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

        //public IActionResult AccessDenied() => View("AccessDenied");
    }

}
