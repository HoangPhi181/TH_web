using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class UpdateProfileDto
    {
        public string avatar { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }
}