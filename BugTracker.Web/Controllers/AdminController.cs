using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Web.Filters;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [NoCache]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
