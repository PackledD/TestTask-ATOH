using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Index(nameof(Login), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Guid { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; }
        [RegularExpression(@"^[ЁёА-яa-zA-Z]+$")]
        public string Name { get; set; }
        [Range(0, 2)]
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } = null;
        public string? ModifiedBy { get; set; } = null;
        public DateTime? RevokedOn { get; set; } = null;
        public string? RevokedBy { get; set; } = null;

        public int Age()
        {
            if (Birthday is null)
            {
                return -1;
            }
            var now = DateTime.UtcNow;
            var age = now.Year - Birthday.Value.Year;
            if (Birthday > now.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}
