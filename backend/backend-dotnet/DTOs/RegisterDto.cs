using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string username { get; set; }

        [Required]
        [EmailAddress] // Đảm bảo email đúng định dạng
        public string email { get; set; }

        [Required]
        [MinLength(6)] // Yêu cầu pass tối thiểu 6 ký tự
        public string password { get; set; }
    }
}