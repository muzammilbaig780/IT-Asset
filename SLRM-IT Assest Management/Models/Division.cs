using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class Division
    {
        [Key]
        public int DivisionId { get; set; }
        [Required]
        [MaxLength(100)]
        public string DivisionName { get; set; }
    }
}
