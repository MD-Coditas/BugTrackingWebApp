using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Web.Controllers
{
    [Authorize(Roles = "QA")]
    public class QAController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
