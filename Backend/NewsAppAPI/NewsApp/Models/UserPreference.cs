using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static NewsApp.Models.Enum;

namespace NewsApp.Models
{
    public class UserPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserPreferenceID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public Preference preference { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
    }


}
