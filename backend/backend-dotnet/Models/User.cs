using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int user_id { get; set; }

        public string username { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }

        [Column("avatar")]
        public string Avatar { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}