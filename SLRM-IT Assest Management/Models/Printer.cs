using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Printer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrinterId { get; set; }

        [Required]
        public string ITAssetTag { get; set; } = null!;

        [Required]
        public string Division { get; set; } = null!;

        
        public int? PrinterTypeId { get; set; }
        public PrinterType? PrinterType { get; set; }

        [Required]
        public int? AssetLocationId { get; set; }
        public AssetLocation? AssetLocation { get; set; }

        [Required]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? PrinterMake { get; set; }
        public string? PrinterModel { get; set; }
        public string? SerialNumber { get; set; }
        public string? CartridgeType { get; set; }

        public string? Status { get; set; }

        [Required]
        public string GRNNumber { get; set; } = null!;

        // 🔴 MUST BE NULLABLE
        public DateTime? GRNDate { get; set; }

        // 🔴 MUST BE NULLABLE
        public DateTime? InvoiceDate { get; set; }

        public string? Warranty { get; set; }

        // 🔴 MUST BE NULLABLE
        public DateTime? EndDate { get; set; }
    }
}
