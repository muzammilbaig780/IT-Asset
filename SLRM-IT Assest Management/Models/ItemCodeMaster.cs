using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class ItemCodeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemCodeMasterId { get; set; }

        public string ItemCode { get; set; }

        [ForeignKey("ItemNameMasterId")]
        public int ItemNameMasterId { get; set; }
        public ItemNameMaster? ItemNameMaster { get; set; }
    }
}
