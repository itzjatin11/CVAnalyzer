using CVAnalyzer.Data;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Linq;

namespace CVAnalyzer.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // /Reports/Cluster
        public IActionResult Cluster()
        {
            var candidates = _context.Candidates.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("StudentClusters");

                // Header
                worksheet.Cell(1, 1).Value = "Student ID";
                worksheet.Cell(1, 2).Value = "Student Name";
                worksheet.Cell(1, 3).Value = "Skills";
                worksheet.Cell(1, 4).Value = "Work Experience";

                // Content
                for (int i = 0; i < candidates.Count; i++)
                {
                    var c = candidates[i];
                    worksheet.Cell(i + 2, 1).Value = c.StudentId;
                    worksheet.Cell(i + 2, 2).Value = c.StudentName;
                    worksheet.Cell(i + 2, 3).Value = c.Skills;
                    worksheet.Cell(i + 2, 4).Value = c.WorkExperience;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Clustered_Student_Report.xlsx"
                    );
                }
            }
        }
    }
}
