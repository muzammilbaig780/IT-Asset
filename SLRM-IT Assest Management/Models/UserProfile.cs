using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }  // Ensure there's a primary key

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Role { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public string ProfilePicturePath { get; set; }
    }
}
