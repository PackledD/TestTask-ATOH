using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        [Key]
        public Guid Guid { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime RevokedOn { get; set; }
        public string RevokedBy { get; set; }
    }
}
