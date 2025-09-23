using System;
using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Asset Name")]
        public string AssetName { get; set; }

        [Required]
        [Display(Name = "Asset Tag")]
        public string AssetTag { get; set; }

        public string Serial { get; set; }

        
        public string ModelNo { get; set; }

        public string Category { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Checked Out To")]
        public DateTime? CheckedOutTo { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Warranty (Months)")]
        public int? Warranty { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expected Checkin Date")]
        public DateTime? ExpectedCheckinDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Next Audit Date")]
        public DateTime? NextAuditDate { get; set; }
    }
}
