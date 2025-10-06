using Microsoft.AspNetCore.Mvc;
using CVAnalyzer.Data;
using CVAnalyzer.Models;
using System;
using System.Linq;
using ClosedXML.Excel;

namespace CVAnalyzer.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }



        public IActionResult ViewCandidates(string filterBy = null, string date = null)
        {
            var candidates = _context.Candidates.AsQueryable();

            DateTime utcToday = DateTime.UtcNow.Date;
            DateTime utcTomorrow = utcToday.AddDays(1);

            if (!string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy.ToLower())
                {
                    case "today":
                        candidates = candidates.Where(c => c.CreatedAt >= utcToday && c.CreatedAt < utcTomorrow);
                        break;

                    case "yesterday":
                        DateTime utcYesterday = utcToday.AddDays(-1);
                        candidates = candidates.Where(c => c.CreatedAt >= utcYesterday && c.CreatedAt < utcToday);
                        break;

                    case "custom":
                        if (DateTime.TryParse(date, out DateTime customDate))
                        {
                            DateTime customDay = customDate.Date;
                            DateTime nextDay = customDay.AddDays(1);
                            candidates = candidates.Where(c => c.CreatedAt >= customDay && c.CreatedAt < nextDay);
                        }
                        break;
                }
            }

            ViewBag.FilterBy = filterBy;
            ViewBag.SelectedDate = date ?? ""; // Default to empty string if no date
            ViewBag.Total = candidates.Count();

            return View("~/Views/CV/ViewCandidates.cshtml", candidates.ToList());
        }


        public IActionResult ExportToExcel(string filterBy = null, string date = null)
        {
            var candidates = _context.Candidates.AsQueryable();

            DateTime utcToday = DateTime.UtcNow.Date;
            DateTime utcTomorrow = utcToday.AddDays(1);

            if (!string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy.ToLower())
                {
                    case "today":
                        candidates = candidates.Where(c => c.CreatedAt >= utcToday && c.CreatedAt < utcTomorrow);
                        break;

                    case "yesterday":
                        DateTime utcYesterday = utcToday.AddDays(-1);
                        candidates = candidates.Where(c => c.CreatedAt >= utcYesterday && c.CreatedAt < utcToday);
                        break;

                    case "custom":
                        if (DateTime.TryParse(date, out DateTime customDate))
                        {
                            DateTime customDay = customDate.Date;
                            DateTime nextDay = customDay.AddDays(1);
                            candidates = candidates.Where(c => c.CreatedAt >= customDay && c.CreatedAt < nextDay);
                        }
                        break;
                }
            }

            var candidateList = candidates.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Candidates");

                // Headers
                worksheet.Cell(1, 1).Value = "Student ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Skills";
                worksheet.Cell(1, 4).Value = "Experience";
                worksheet.Cell(1, 5).Value = "Category";
                worksheet.Cell(1, 6).Value = "Created At (UTC)";

                int currentRow = 2;
                foreach (var c in candidateList)
                {
                    worksheet.Cell(currentRow, 1).Value = c.StudentId;
                    worksheet.Cell(currentRow, 2).Value = c.StudentName;
                    worksheet.Cell(currentRow, 3).Value = c.Skills;
                    worksheet.Cell(currentRow, 4).Value = c.WorkExperience;
                    worksheet.Cell(currentRow, 5).Value = c.Category;
                    worksheet.Cell(currentRow, 6).Value = c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    currentRow++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "CandidatesReport.xlsx");
                }
            }
        }

    }
}