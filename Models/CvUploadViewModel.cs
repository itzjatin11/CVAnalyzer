using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CVAnalyzer.Models
{

    public class CvUploadViewModel
    {
        [Required]
        public IFormFile CvFile { get; set; }
    }
}