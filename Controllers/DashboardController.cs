using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using CVAnalyzer.Data;
using CVAnalyzer.Models;

namespace CVAnalyzer.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var candidates = _context.Candidates.ToList();

            // Basic skill-based grouping
            var clusters = candidates
                .GroupBy(c => GetSkillClusterName(c.Skills))
                .SelectMany(g => g.Select(c => new ClusterGroupViewModel
                {
                    ClusterName = g.Key,
                    StudentId = c.StudentId,
                    StudentName = c.StudentName,
                    Skills = c.Skills
                }))
                .ToList();

            var model = new DashboardViewModel
            {
                StudentClusters = clusters
            };

            return View(model);
        }

        // Simple skill group name based on keywords
        private string GetSkillClusterName(string skills)
        {
            skills = skills.ToLower();

            if (skills.Contains("python") || skills.Contains("ml") || skills.Contains("ai"))
                return "AI / ML";

            if (skills.Contains("html") || skills.Contains("css") || skills.Contains("javascript"))
                return "Web Development";

            if (skills.Contains("sql") || skills.Contains("excel") || skills.Contains("data"))
                return "Data / Analytics";

            return "General";
        }
    }
}
