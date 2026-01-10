namespace SLRM_IT_Assest_Management.Models
{
    public class DashboardViewModel
    {
        public int AssetCount { get; set; }
        public int LicenseCount { get; set; }
        public int ConsumablesCount { get; set; }
        public User LoggedInUser { get; set; }

        // Overall Asset Status
        public int ReadyToDeployCount { get; set; }
        public int ActiveCount { get; set; }
        public int NotActiveCount { get; set; }
        public int ScrapCount { get; set; }
        //public int NACount { get; set; }

        // Device Type Counts
        public int LaptopCount { get; set; }
        public int DesktopCount { get; set; }

        //// Laptop-specific status counts
        public int LaptopWorking { get; set; }
        public int LaptopUnderRepair { get; set; }
        public int LaptopScrap { get; set; }
        public int LaptopMissing { get; set; }

        //// Desktop-specific status counts
        public int DesktopWorking { get; set; }
        public int DesktopUnderRepair { get; set; }
        public int DesktopScrap { get; set; }
        public int DesktopMissing { get; set; }




    }
}
