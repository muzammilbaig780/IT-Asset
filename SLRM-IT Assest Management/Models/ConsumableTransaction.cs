using System;
using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class ConsumableTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int ConsumableId { get; set; }

        public ConsumableTransactionType TransactionType { get; set; }

        public decimal Quantity { get; set; }

        public int? AssetId { get; set; }

        public DateTime TransactionDate { get; set; }

        public string PerformedBy { get; set; }
        public string ReferenceNo { get; set; }

        public string Remarks { get; set; }

        /* Navigation */
        public Consumable Consumable { get; set; }
        public Asset Asset { get; set; }
    }
}
    