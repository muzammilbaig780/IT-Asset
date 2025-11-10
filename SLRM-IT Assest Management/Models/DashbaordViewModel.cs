namespace SLRM_IT_Assest_Management.Models
{
    public class DashboardViewModel
    {
        public int AssetCount { get; set; }
        public int LicenseCount { get; set; }
        //public int ActiveCount { get; set; }
        //public int NotActiveCount { get; set; }
        public User LoggedInUser { get; set; }

        // Deployment / Asset Status
        public int ReadyToDeployCount { get; set; }
        public int ActiveCount { get; set; }
        public int NotActiveCount { get; set; }
        public int ScrapCount { get; set; }
        public int NACount { get; set; }

        public int LaptopCount { get; set; }
        public int DesktopCount { get; set; }
    }

}
