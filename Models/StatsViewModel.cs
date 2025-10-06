using System.Collections.Generic;

namespace CVAnalyzer.Models
{
    public class StatsViewModel
    {
        public int TotalCVsUploaded { get; set; }
        public int UniqueCandidates { get; set; }
        public string MostCommonSkill { get; set; }
        public int CVsProcessedToday { get; set; }

        public Dictionary<string, int> SkillDistribution { get; set; } = new();
    }
}
