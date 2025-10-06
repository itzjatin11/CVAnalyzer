using CVAnalyzer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using CVAnalyzer.Models;
using CVAnalyzer.Services;

namespace CVAnalyzer.Controllers
{
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly CvParserService _parser;

        public UploadController(IWebHostEnvironment env, ApplicationDbContext context, CvParserService parser)
        {
            _env = env;
            _context = context;
            _parser = parser;
        }

        [HttpGet]
        public IActionResult Single()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Single(CvUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                string ext = Path.GetExtension(model.CvFile.FileName).ToLower();

                if (ext != ".pdf" && ext != ".docx")
                {
                    TempData["Success"] = "❌ Only PDF and DOCX files are allowed.";
                    return RedirectToAction("Single");
                }

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, model.CvFile.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CvFile.CopyToAsync(fileStream);
                }

                // Extract and save to DB
                var candidate = _parser.ExtractFromFile(filePath);

                if (!string.IsNullOrWhiteSpace(candidate.StudentId))
                {
                    _context.Candidates.Add(candidate);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "✅ CV uploaded and candidate details saved!";
                }
                else
                {
                    TempData["Success"] = "⚠️ CV uploaded, but no details found in the file.";
                }

                return RedirectToAction("Single");
            }

            return View(model);
        }
    }
}
