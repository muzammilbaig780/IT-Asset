using System;
using System.Collections.Generic;

namespace SLRM_IT_Assest_Management.ViewModels
{
    public class AssetHistoryVM
    {
        public int AssetId { get; set; }
        public string AssetTag { get; set; }
        public string UserName { get; set; }
        public string HostName { get; set; }

        // Consumables used
        public List<ConsumableHistory> Consumables { get; set; } = new();

        // Components installed
        public List<ComponentHistory> Components { get; set; } = new();

        // Accessories linked
        public List<AccessoryHistory> Accessories { get; set; } = new();
    }

    public class ConsumableHistory
    {
        public string ConsumableName { get; set; }
        public decimal Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string PerformedBy { get; set; }
    }

    public class ComponentHistory
    {
        public string ComponentName { get; set; }
        public string SerialNo { get; set; }
        public DateTime InstalledOn { get; set; }
    }

    public class AccessoryHistory
    {
        public string AccessoryName { get; set; }
        public string SerialNo { get; set; }
        public DateTime AssignedOn { get; set; }
    }
}
