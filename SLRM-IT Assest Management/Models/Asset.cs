using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetId { get; set; }

        public int SlNo { get; set; }  // Auto-generate in controller

        // --- Foreign Keys (Nullable for safe binding) ---
        [Required(ErrorMessage = "Please select an Asset Type.")]
        [Display(Name = "Asset Type")]
        public int? AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType? AssetType { get; set; }

        [Required(ErrorMessage = "Please select a Company.")]
        [Display(Name = "Company")]
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        [Required(ErrorMessage = "Please select a Status.")]
        public int? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status? AssetStatus { get; set; }

        [Required(ErrorMessage = "Please select a Location.")]
        public int? AssetLocationId { get; set; }
        [ForeignKey("AssetLocationId")]
        public AssetLocation? AssetLocation { get; set; }

        // --- Required Fields ---
        [Required(ErrorMessage = "Department is required.")]
        public string? Department { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Emp Code is required.")]
        public string? EmpCode { get; set; }

        [Required(ErrorMessage = "Host Name is required.")]
        public string? HostName { get; set; }

        [Required(ErrorMessage = "Block is required.")]
        public string? Block { get; set; }

        [Required(ErrorMessage = "Asset Tag is required.")]
        public string? AssetTag { get; set; }

        [Required(ErrorMessage = "Make is required.")]
        public string? Make { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        public string? Model { get; set; }

        [Required(ErrorMessage = "Monitor Make is required.")]
        public string? MoniterMake { get; set; }

        [Required(ErrorMessage = "Monitor Model is required.")]
        public string? MoniterModel { get; set; }

        [Required(ErrorMessage = "Serial No is required.")]
        public string? SerialNo { get; set; }

        [Required(ErrorMessage = "Processor is required.")]
        public string? Processor { get; set; }

        [Required(ErrorMessage = "RAM is required.")]
        public string? Ram { get; set; }

        [Required(ErrorMessage = "HDD is required.")]
        public string? Hdd { get; set; }

        [Required(ErrorMessage = "Division is required.")]
        public string? Division { get; set; }

        [Required(ErrorMessage = "Antivirus info is required.")]
        public string? AntiVirus { get; set; }

        [Required(ErrorMessage = "OS Version is required.")]
        public string? OSVersion { get; set; }

        [Required(ErrorMessage = "AutoCAD version is required.")]
        public string? AutoCad { get; set; }

        [Required(ErrorMessage = "Office version is required.")]
        public string? Office { get; set; }

        [Required(ErrorMessage = "Windows License Key is required.")]
        public string? WindowLicenseKey { get; set; }

        [Required(ErrorMessage = "IP Address is required.")]
        public string? IPAddress { get; set; }

        [Required(ErrorMessage = "Nitro info is required.")]
        public string? Nitro { get; set; }

        [Required(ErrorMessage = "Audit Status is required.")]
        public string? AuditStatus { get; set; }
    }
}
