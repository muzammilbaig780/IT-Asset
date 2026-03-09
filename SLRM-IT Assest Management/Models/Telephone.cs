using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Telephone
    {
        [Key]
        public int TelephoneId { get; set; }
        public string UserName { get; set; }
        public  string TelephoneNo { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        public string  Connection { get; set; }
    }
}
