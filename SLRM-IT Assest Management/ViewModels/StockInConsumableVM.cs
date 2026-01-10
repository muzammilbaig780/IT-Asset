using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.ViewModels
{
    public class StockInConsumableVM
    {
        [Required(ErrorMessage = "Please select a consumable")]
        public int? ConsumableId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Reference number is required")]
        public string ReferenceNo { get; set; }

        public List<SelectListItem> Consumables { get; set; }
    }
}
