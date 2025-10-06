using Microsoft.AspNetCore.Mvc;
using CVAnalyzer.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using CVAnalyzer.Models;

namespace CVAnalyzer.Controllers
{
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var candidates = _context.Candidates.ToList();

            var totalCVs = candidates.Count;

            var uniqueCandidates = candidates
                .Select(c => c.StudentId)
                .Distinct()
                .Count();

            // Skill frequency processing
            var skillFrequency = candidates
                .Where(c => !string.IsNullOrWhiteSpace(c.Skills))
                .SelectMany(c => c.Skills.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim().ToLower())
                .GroupBy(s => s)
                .Select(g => new { Skill = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            var mostCommonSkill = skillFrequency.FirstOrDefault()?.Skill ?? "N/A";

            var skillDistribution = skillFrequency
                .ToDictionary(x => x.Skill, x => x.Count);

            var cvsProcessedToday = candidates
                .Count(c => c.CreatedAt.Date == DateTime.UtcNow.Date);

            var model = new StatsViewModel
            {
                TotalCVsUploaded = totalCVs,
                UniqueCandidates = uniqueCandidates,
                MostCommonSkill = mostCommonSkill,
                CVsProcessedToday = cvsProcessedToday,
                SkillDistribution = skillDistribution
            };

            return View(model);
        }
    }
}
