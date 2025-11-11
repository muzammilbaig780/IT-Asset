using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class Department
    {
        internal readonly int Id;

        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
