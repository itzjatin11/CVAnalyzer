using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;

namespace CVAnalyzer.Services
{
    public class AffindaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AffindaService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _apiKey = config["Affinda:ApiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<(string StudentId, string StudentName, string Skills, string Experience)> ExtractFromFileAsync(string filePath)
        {
            var url = "https://api.affinda.com/v2/resumes";

            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            form.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync(url, form);
            if (!response.IsSuccessStatusCode)
                return ("Not Provided", "Unknown", "Not listed", "Not listed");

            var json = await response.Content.ReadAsStringAsync();

            // Debug log the full response
            var logPath = Path.Combine("wwwroot", "logs", "affinda_raw_response.json");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            File.WriteAllText(logPath, json);

            var doc = JsonDocument.Parse(json);
            var resume = doc.RootElement.GetProperty("data");

            // Safely extract the name field
            string name = "Unknown";
            if (resume.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.Object)
            {
                if (nameProp.TryGetProperty("raw", out var rawName) && rawName.ValueKind == JsonValueKind.String)
                {
                    name = rawName.GetString();
                }
                else if (nameProp.TryGetProperty("formatted", out var formatted) && formatted.ValueKind == JsonValueKind.Object)
                {
                    var first = formatted.TryGetProperty("first", out var f) ? f.GetString() : "";
                    var last = formatted.TryGetProperty("last", out var l) ? l.GetString() : "";
                    name = $"{first} {last}".Trim();
                }
            }

            string studentId = "Not Provided";
            string skills = ExtractSkills(resume);
            string experience = ExtractExperience(resume);

            return (studentId, name, skills, experience);
        }

        private static string ExtractSkills(JsonElement resume)
        {
            if (!resume.TryGetProperty("skills", out var skillsArray) || skillsArray.ValueKind != JsonValueKind.Array)
                return "Not listed";

            var skills = new List<string>();
            foreach (var skill in skillsArray.EnumerateArray())
            {
                if (skill.TryGetProperty("name", out var name))
                    skills.Add(name.GetString());
            }

            return skills.Count > 0 ? string.Join(", ", skills) : "Not listed";
        }

        private static string ExtractExperience(JsonElement resume)
        {
            if (!resume.TryGetProperty("workExperience", out var workArray) || workArray.ValueKind != JsonValueKind.Array)
                return "Not listed";

            var jobs = new List<string>();
            foreach (var job in workArray.EnumerateArray())
            {
                var title = job.TryGetProperty("jobTitle", out var t) ? t.GetString() : null;
                var org = job.TryGetProperty("organization", out var o) ? o.GetString() : null;

                if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(org))
                    jobs.Add($"{title}, {org}");
            }

            return jobs.Count > 0 ? string.Join("; ", jobs) : "Not listed";
        }
    }
}
