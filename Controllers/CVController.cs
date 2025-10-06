using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CVAnalyzer.Models;
using CVAnalyzer.Data;
using CVAnalyzer.Services;
using Microsoft.AspNetCore.Hosting;
using CVAnalyzer.Helpers;

namespace CVAnalyzer.Controllers
{
    public class CVController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CvParserService _cvParserService;
        private readonly IWebHostEnvironment _environment;

        public CVController(ApplicationDbContext context, CvParserService cvParserService, IWebHostEnvironment environment)
        {
            _context = context;
            _cvParserService = cvParserService;
            _environment = environment;
        }

        // GET: Upload View
        public IActionResult Upload()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Upload CV(s)
        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ViewBag.Message = "No files selected.";
                return View();
            }

            var uploadPath = Path.Combine(_environment.WebRootPath, "UploadedCVs");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var file in files)
            {
                var filePath = Path.Combine(uploadPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var candidate = _cvParserService.ExtractFromFile(filePath);
                _context.Candidates.Add(candidate);
            }

            await _context.SaveChangesAsync();

            ViewBag.Message = "CV(s) uploaded and processed successfully.";
            return View();
        }

        // Group candidates by weighted classifier category
        public IActionResult SkillClusters()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var candidates = _context.Candidates.ToList();
            var categorizedGroups = new Dictionary<string, List<Candidate>>(StringComparer.OrdinalIgnoreCase);

            foreach (var candidate in candidates)
            {
                var skillsList = _cvParserService.ParseSkills(candidate.Skills);
                var matchedGroups = new SkillClassifier().ClassifyPerson(skillsList);

                // Only group if they meet the threshold
                if (!matchedGroups.Any())
                    continue;

                foreach (var group in matchedGroups)
                {
                    if (!categorizedGroups.ContainsKey(group))
                        categorizedGroups[group] = new List<Candidate>();

                    if (!categorizedGroups[group].Any(c =>
                        c.StudentId == candidate.StudentId &&
                        c.StudentName.Equals(candidate.StudentName, StringComparison.OrdinalIgnoreCase)))
                    {
                        categorizedGroups[group].Add(candidate);
                    }
                }
            }

            return View("SkillClusters", categorizedGroups);
        }

        public IActionResult ViewCandidates()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var candidates = _context.Candidates.ToList();
            return View(candidates);
        }
    }
}
