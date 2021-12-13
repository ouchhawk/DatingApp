using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        // girilmezse hata verecek
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength =4 )]
        public string Password { get; set; }
    }
}