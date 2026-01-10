using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class ITAssetDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ITAssetDetailId { get; set; }

        public int SlNo { get; set; }
        public string? UserName { get; set; }
        public string? Department { get; set; }
        public string? Division { get; set; }
        public string? AssetLocation { get; set; }
        public string? AssetType { get; set; }
        public string? Status { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? SerialNo { get; set; }

        [Display(Name = "Telephone No")]
        [MaxLength(20)]
        public string? TelephoneNo { get; set; }

        [Display(Name = "Parallel Connection")]
        [MaxLength(50)]
        public string? ParallelConnection { get; set; }

        [Display(Name = "Screen Size")]
        [MaxLength(20)]
        public string? ScreenSize { get; set; }

        [Display(Name = "Frequency No")]
        [MaxLength(20)]
        public string? FrequencyNo { get; set; }

        [Display(Name = "License No")]
        [MaxLength(50)]
        public string? LicenseNo { get; set; }

        [Display(Name = "Ports")]
        [MaxLength(100)]
        public string? Ports { get; set; }

        // ------------------ ADD THESE ------------------
        [ForeignKey("Asset")]
        public int AssetId { get; set; }        // Link to Asset table
        public Asset Asset { get; set; }        // Navigation property

        // New fields for Asset History
        public string ComponentName { get; set; }    // e.g., RAM, CPU, HDD
        public DateTime InstallDate { get; set; }
    }
}
