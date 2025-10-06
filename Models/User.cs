using System.ComponentModel.DataAnnotations;

namespace CVAnalyzer.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Username must contain only letters.")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; } = "User";
    }
}
