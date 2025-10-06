using System;
using System.Collections.Generic;
using System.Linq;

namespace CVAnalyzer.Helpers
{
    public class SkillClassifier
    {
        // Define available categories and their weighted skills
        private readonly Dictionary<string, Dictionary<string, int>> categorySkillMap =
    new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase)
{
    {
        "Developer", new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Java", 5 },
            { "C#", 5 },
            { "Python", 5 },
            { "Git", 4 },
            { "SQL", 3 },
            { "JavaScript", 3 },
            { "Kotlin", 4 },
            { "Android", 3 },
            { "Linux", 3 },
            { "Docker", 4 },
            { "AWS", 4 },
            { "Azure", 4 },
            { "React", 4 },
            { "Node.js", 4 }
        }
    },
    {
        "Designer", new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Photoshop", 5 },
            { "Illustrator", 5 },
            { "Figma", 5 },
            { "UI Design", 4 },
            { "UX", 4 },
            { "Sketch", 4 },
            { "InDesign", 4 },
            { "CorelDRAW", 3 },
            { "After Effects", 3 },
            { "Animation", 4 }
        }
    },
    {
        "Data Scientist", new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Python", 5 },
            { "R", 5 },
            { "Machine Learning", 5 },
            { "Data Analysis", 4 },
            { "SQL", 3 },
            { "TensorFlow", 4 },
            { "Pandas", 4 },
            { "Statistics", 4 },
            { "Deep Learning", 5 },
            { "Data Visualization", 4 }
        }
    },
    {
        "Business Analyst", new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Business Analysis", 5 },
            { "Stakeholder Management", 4 },
            { "Process Improvement", 4 },
            { "Requirements Gathering", 5 },
            { "Agile", 4 },
            { "Scrum", 4 },
            { "UML", 3 },
            { "Data Modeling", 4 },
            { "Microsoft Excel", 3 },
            { "Power BI", 3 }
        }
    },
    {
        "Marketing Specialist", new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "SEO", 5 },
            { "Google Analytics", 4 },
            { "Social Media Marketing", 5 },
            { "Content Marketing", 4 },
            { "Facebook Ads", 3 },
            { "Google Ads", 3 },
            { "Email Marketing", 4 },
            { "Copywriting", 4 },
            { "Brand Management", 4 }
        }
    },
    {
        "Hospitality Manager", new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Customer Service", 5 },
            { "Event Planning", 4 },
            { "Hospitality Management", 5 },
            { "Booking Systems", 4 },
            { "Inventory Management", 4 },
            { "Team Leadership", 4 },
            { "Budgeting", 3 },
            { "Food Safety", 4 },
            { "Housekeeping", 3 }
        }
    },
    {
        "Finance Professional", new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Accounting", 5 },
            { "Financial Analysis", 5 },
            { "Excel", 4 },
            { "QuickBooks", 4 },
            { "Taxation", 4 },
            { "Auditing", 4 },
            { "Budgeting", 4 },
            { "Risk Management", 4 },
            { "Financial Reporting", 5 }
        }
    }
    // Add more categories if needed
};

        // Define category thresholds for the new categories
        private readonly Dictionary<string, int> categoryThresholds =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
    { "Developer", 8 },
    { "Designer", 8 },
    { "Data Scientist", 8 },
    { "Business Analyst", 8 },
    { "Marketing Specialist", 8 },
    { "Hospitality Manager", 8 },
    { "Finance Professional", 8 }
        };


        /// <summary>
        /// Classifies based on a list of skills (weighted scoring + thresholds).
        /// </summary>

        public List<string> ClassifyPerson(List<string> personSkills)
        {
            var results = new List<string>();

            // Normalize skills: lowercase and trim, remove duplicates
            var normalizedSkills = personSkills
                .Select(s => s.ToLower().Split('(')[0].Trim())
                .Distinct()
                .ToList();

            foreach (var category in categorySkillMap)
            {
                int score = 0;

                foreach (var catSkill in category.Value.Keys)
                {
                    var normalizedCatSkill = catSkill.ToLower();

                    // Check for exact skill match or word boundary match instead of simple Contains
                    bool isMatch = normalizedSkills.Any(inputSkill =>
                        inputSkill.Equals(normalizedCatSkill, StringComparison.OrdinalIgnoreCase) || // exact match
                        inputSkill.Split(' ').Any(word => word.Equals(normalizedCatSkill, StringComparison.OrdinalIgnoreCase)) // word boundary match
                    );

                    if (isMatch)
                    {
                        score += category.Value[catSkill];
                    }
                }

                if (score >= categoryThresholds[category.Key])
                {
                    results.Add(category.Key);
                }
            }

            return results;
        }



        /// <summary>
        /// Overload for raw CV text input (splits into tokens & matches partially).
        /// </summary>
        public List<string> ClassifyPerson(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return new List<string>();

            var tokens = rawText.Split(new[] { ',', ';', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(t => t.Trim())
                                .Where(t => !string.IsNullOrEmpty(t))
                                .ToList();

            return ClassifyPerson(tokens);
        }

        /// <summary>
        /// Discovers dynamic skill-based groups from data.
        /// </summary>
        public List<string> DiscoverDynamicGroups(List<List<string>> allProfiles, int minMembers = 3)
        {
            var skillFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var profile in allProfiles)
            {
                foreach (var skill in profile.Distinct())
                {
                    if (!skillFrequency.ContainsKey(skill))
                        skillFrequency[skill] = 0;

                    skillFrequency[skill]++;
                }
            }

            return skillFrequency
                .Where(kvp => kvp.Value >= minMembers)
                .Select(kvp => $"Group: {kvp.Key} ({kvp.Value} members)")
                .ToList();
        }
    }
}
