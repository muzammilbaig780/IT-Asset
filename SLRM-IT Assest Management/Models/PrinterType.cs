using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class PrinterType
    {
        [Key]
        public int PrinterTypeId { get; set; }  // Primary Key for the PrinterType

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;  // Name of the Printer Type (e.g., Laser, Inkjet, etc.)
    }
}
