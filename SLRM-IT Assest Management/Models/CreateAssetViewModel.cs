using SLRM_IT_Assest_Management.Models;
using System.Collections.Generic;

namespace SLRM_IT_Assest_Management.ViewModels
{
    public class CreateAssetViewModel
    {
        public Asset Asset { get; set; }

        public List<AssetType>? AssetTypes { get; set; }
        public List<Status>? AssetStatuses { get; set; }
        public List<AssetLocation>? AssetLocations { get; set; }
        public List<Company>? Companies { get; set; }
    }
}
