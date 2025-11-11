using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models

{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required, Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        // Foreign key
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; } // navigation property

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePicturePath { get; set; }
    }


}

