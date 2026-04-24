using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        public int id { get; set; }

        public string account_id { get; set; }
        public int user_id { get; set; }
        public double balance { get; set; }
        public double used_margin { get; set; }
        public int leverage { get; set; }
        public string typeAccount { get; set; }
    }
}