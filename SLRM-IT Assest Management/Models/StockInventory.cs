using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class StockInventory
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoreInventoryId { get; set; }

        [Required(ErrorMessage = "Please select a ItemName.")]
        public int? ItemNameMasterId { get; set; }
        [ForeignKey("ItemNameMasterId")]
        public ItemNameMaster? ItemNameMaster { get; set; }

        [Required(ErrorMessage = "Please select a ItemCode.")]
        public int? ItemCodeMasterId { get; set; }
        [ForeignKey("ItemCodeMasterId")]
        public ItemCodeMaster? ItemCodeMaster { get; set; }

        public string? Category { get; set; }

        public string? GRNNumber { get; set; }
        public string RequesitionNo { get; set; }

        public int ReceivedQty { get; set; }

        public int AvailableQty { get; set; }


        public string? StoreLocation { get; set; }

        public string? ReceivedBy { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; } = string.Empty;


        [NotMapped]
        public string ItemDisplay
        {
            get
            {
                string itemName = ItemNameMaster?.ItemName ?? "";
                string itemCode = ItemCodeMaster?.ItemCode ?? "";

                return itemName + " - " + itemCode;
            }
        }

    }
}
