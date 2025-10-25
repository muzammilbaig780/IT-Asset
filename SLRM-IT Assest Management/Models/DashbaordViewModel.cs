namespace SLRM_IT_Assest_Management.Models
{
    public class DashboardViewModel
    {
        public int AssetCount { get; set; }
        public int LicenseCount { get; set; }

        // Deployment / Asset Status
        public int ReadyToDeployCount { get; set; }
        public int ActiveCount { get; set; }
        public int NotActiveCount { get; set; }
    }

}
