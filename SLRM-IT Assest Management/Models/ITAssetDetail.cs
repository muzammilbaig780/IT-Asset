using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class ITAssetDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ITAssetDetailId { get; set; }


        [Required]
        [Display(Name = "Asset")]
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

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
    }
}
