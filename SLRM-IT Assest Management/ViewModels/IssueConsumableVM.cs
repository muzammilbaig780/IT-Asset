using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SLRM_IT_Assest_Management.ViewModels
{
    public class IssueConsumableVM
    {
        public int ConsumableId { get; set; }
        public decimal Quantity { get; set; }
        public int AssetId { get; set; }

        public IEnumerable<SelectListItem> Consumables { get; set; }
        public IEnumerable<SelectListItem> Assets { get; set; }
    }
}
