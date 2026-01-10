    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    namespace SLRM_IT_Assest_Management.Models
    {
        public class Consumable
        {
            [Key]
            public int ConsumableId { get; set; }

            [Required]
            [StringLength(50)]
            public string ConsumableCode { get; set; }

            [Required]
            [StringLength(150)]
            public string ConsumableName { get; set; }
            public decimal Quantity { get; set; } = 0;

            [StringLength(50)]
            public string Category { get; set; }   // Toner, Cable, Ink

            [StringLength(20)]
            public string Unit { get; set; }       // Nos, Meter

            public int ReorderLevel { get; set; }

            public bool IsActive { get; set; }

            /* Navigation */
            public virtual ConsumableStock? Stock { get; set; }
        public virtual ICollection<ConsumableTransaction> Transactions { get; set; } = new List<ConsumableTransaction>();

    }
}