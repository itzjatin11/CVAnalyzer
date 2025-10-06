using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // 👇 Redirect to Login if not authenticated
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            return View(); // Load dashboard
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session variables
            return RedirectToAction("Login", "Account");
        }
        [Authorize]
        public IActionResult Upload() => View();

        [Authorize]
        public IActionResult ViewCandidates() => View();

        [Authorize]
        public IActionResult SkillClusters() => View();

        [Authorize]
        public IActionResult Stats() => View();




    }
}
