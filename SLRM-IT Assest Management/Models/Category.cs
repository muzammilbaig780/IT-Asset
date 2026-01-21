using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
