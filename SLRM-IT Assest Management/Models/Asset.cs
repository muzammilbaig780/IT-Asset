using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SLRM_IT_Assest_Management.Models
{
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetId { get; set; }
        public int SlNo { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        public string EmpCode { get; set; }
        public string HostName { get; set; }
        public string Block { get; set; }
        public string AssetLocation { get; set; }
        public string AssetTag { get; set; }

        [Display(Name = "Make")]
        public string Make { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }
        public string MoniterMake { get; set; }
        public string MoniterModel { get; set; }

        [Display(Name = "SerialNo")]
        public string SerialNo { get; set; }

        public string Processor { get; set; }

        public string Ram { get; set; }
        public string Hdd { get; set; }
        public string Division { get; set; }
        public string AntiVirus { get; set; }
        public string Status { get; set; }
        public string OSVersion { get; set; }
        public string AutoCad { get; set; }
        public string Office { get; set; }
        public string WindowLicenseKey { get; set; }
        public string IPAddress { get; set; }
        public string Nitro { get; set; }
        public string AuditStatus { get; set; }

    }
}
