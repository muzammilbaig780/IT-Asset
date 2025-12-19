using AssetManagement.Data;
using SLRM_IT_Assest_Management.Models;
using System.Collections.Generic;

namespace SLRM_IT_Assest_Management.ViewModels
{
    public class CreateAssetViewModel
    {
        public Asset Asset { get; set; }

        // Use fully qualified names to avoid ambiguity
        public List<AssetType>? AssetTypes { get; set; }

        // Specify the correct namespace for the Status class
        public List<SLRM_IT_Assest_Management.Models.Status>? AssetStatuses { get; set; }

        public List<AssetLocation>? AssetLocations { get; set; }
        public List<Company>? Companies { get; set; }
    }
}
