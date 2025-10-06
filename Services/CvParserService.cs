using CVAnalyzer.Models;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CVAnalyzer.Helpers;

namespace CVAnalyzer.Services
{
    public class CvParserService
    {
        private readonly AffindaService _affindaService;
        private readonly SkillClassifier _skillClassifier;

        public CvParserService(AffindaService affindaService, SkillClassifier skillClassifier)
        {
            _affindaService = affindaService;
            _skillClassifier = skillClassifier;
        }

        public Candidate ExtractFromFile(string filePath)
        {
            var result = _affindaService.ExtractFromFileAsync(filePath).Result;

            var skillsList = ParseSkills(result.Skills);

            // Weighted classifier decides the category
            var categories = _skillClassifier.ClassifyPerson(skillsList);

            return new Candidate
            {
                StudentId = string.IsNullOrWhiteSpace(result.StudentId) ? "Not Provided" : result.StudentId,
                StudentName = string.IsNullOrWhiteSpace(result.StudentName) ? "Unknown" : result.StudentName,
                Skills = string.IsNullOrWhiteSpace(result.Skills) ? "Not listed" : result.Skills,
                WorkExperience = string.IsNullOrWhiteSpace(result.Experience) ? "Not listed" : result.Experience,
                FileName = Path.GetFileName(filePath),
                Category = categories.Any() ? string.Join(", ", categories) : "Uncategorized"
            };
        }

        public List<string> ParseSkills(string skillsString)
        {
            return string.IsNullOrWhiteSpace(skillsString)
                ? new List<string>()
                : skillsString
                    .Split(new[] { ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
        }
    }
}
