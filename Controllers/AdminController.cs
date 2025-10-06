using CVAnalyzer.Data;
using CVAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CVAnalyzer.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Only allow access if session Role is Admin
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var users = _db.Users.ToList();
            return View(users);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                user.Role = "User";  // Auto-assign role "User"

                // You currently store plain passwords, consider hashing here if you want to improve security.
                _db.Users.Add(user);
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Edit/5
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingUser = _db.Users.Find(id);
                if (existingUser == null) return NotFound();

                existingUser.Username = user.Username;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = user.Password; // Add hashing if desired
                }

                // Role remains unchanged here

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Delete/5
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
