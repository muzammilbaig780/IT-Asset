using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class Cctv
    {
        [Key]
        public int CctvId { get; set; } // Primary key

        public string ITAssetTag { get; set; }              // IT ASSET TAG
        public string DeviceType { get; set; }             // Device Type
        public string Model { get; set; }                  // MODEL
        public string CameraSerialNumber { get; set; }     // CAMERA SERIAL NUMBER
        public string HardDiskCapacity { get; set; }       // HARD DISK CAPACITY
        public int? NumberOfHardDisks { get; set; }         // NUMBER OF HARD DISK
        public string Channel { get; set; }                // Channel
        [Required]
        public int? AssetLocationId { get; set; }
        public AssetLocation? AssetLocation { get; set; }

        [Required]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public DateTime? HandoverDate { get; set; }        // Handover Date
        public string Status { get; set; }                 // Status
        public DateTime? InvoiceDate { get; set; }         // Invoice date
        public DateTime? GRNDate { get; set; }             // GRN Date
        public string GRNNumber { get; set; }              // GRN number
        public string Warranty { get; set; }               // warranty
        public DateTime? EndDate { get; set; }             // End Date
        public DateTime? ScrapDate { get; set; }           // Scrap Date
        public string Remark { get; set; }                 // Remark
    }
}
