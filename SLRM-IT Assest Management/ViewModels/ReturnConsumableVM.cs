using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.ViewModels
{
    public class ReturnConsumableVM
    {
        public int ConsumableId { get; set; }
        public string ConsumableName { get; set; }
        public decimal MaxQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
    }

}
