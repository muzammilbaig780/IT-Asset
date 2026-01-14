using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class Barcode
    {
    

        [Key]
        public int BarcodeId { get; set; } // Primary key

        public string ITAssetTag { get; set; }              // IT ASSET TAG
        public string Make {  get; set; }
        public string Model { get; set; }                  // MODEL
        public string SerialNumber { get; set; }     //  SERIAL NUMBER
        
        
        [Required]
        public int? AssetLocationId { get; set; }
        public AssetLocation? AssetLocation { get; set; }
        [Required]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string Status { get; set; }                 // Status
        public DateTime? InvoiceDate { get; set; }         // Invoice date
        public DateTime? GRNDate { get; set; }             // GRN Date

        [Display(Name = "GRN Number")]
        public string? GRNNumber { get; set; }
        // GRN number
        public string Warranty { get; set; }               // warranty
        public DateTime? EndDate { get; set; }             // End Date
        public DateTime? ScrapDate { get; set; }           // Scrap Date
        public string Remark { get; set; }                 // Remark
    }
}
