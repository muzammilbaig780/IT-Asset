using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Accessory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccessoryId { get; set; }

        [Required]
        public string Name { get; set; }              // Accessory Name

        public string SerialNo { get; set; }          // Serial Number, optional

        [Required]
        public int AssetId { get; set; }              // Link to Asset

        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }              // Navigation property

        public DateTime AssignedOn { get; set; }      // When assigned to asset
    }
}
