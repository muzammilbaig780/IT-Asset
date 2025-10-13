using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class AssetLocation
    {
        [Key]
        public int AssetLocationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
