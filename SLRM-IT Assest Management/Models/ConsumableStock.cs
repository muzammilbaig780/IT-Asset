using SLRM_IT_Assest_Management.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SLRM_IT_Assest_Management.Models
{
    public class ConsumableStock
    {
        [Key, ForeignKey("Consumable")]
        public int? ConsumableId { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal AvailableQuantity { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        /* Navigation */
        public virtual Consumable Consumable { get; set; }
    }
}


