using System;
using System.ComponentModel.DataAnnotations;

namespace CVAnalyzer.Models
{
    public class Candidate
    {
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string Skills { get; set; } // comma-separated string

        public string WorkExperience { get; set; }

        public string FileName { get; set; }

        public string Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // New: for tracking stats
    }
}
