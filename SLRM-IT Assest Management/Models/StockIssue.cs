using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class StockIssue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IssueId { get; set; }

        // Item
        [Required]
        public int StoreInventoryId { get; set; }

        [ForeignKey(nameof(StoreInventoryId))]
        public StockInventory? StockInventory { get; set; }

        // Department
        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        // Employee Details
        [Required]
        public string EmployeeName { get; set; } = string.Empty;

        public string EmployeeCode { get; set; } = string.Empty;

        // Issue Details
        [Required]
        public int IssueQty { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string RequestedBy { get; set; } = string.Empty;

        // ============================
        // Level 1 Approval (Manager)
        // ============================

        public string Level1Status { get; set; } = "Pending";

        public string? Level1ApprovedBy { get; set; }

        public DateTime? Level1ApprovedDate { get; set; }

        public string? Level1Remarks { get; set; }

        // ============================
        // Level 2 Approval (HOD / MIS Head)
        // ============================

        public string Level2Status { get; set; } = "Pending";

        public string? Level2ApprovedBy { get; set; }

        public DateTime? Level2ApprovedDate { get; set; }

        public string? Level2Remarks { get; set; }

        // ============================
        // Final Issue Status
        // ============================

        public string Status { get; set; } = "Pending";

        public DateTime? IssuedDate { get; set; }

        public string? IssuedBy { get; set; }

        public string? Remarks { get; set; }
    }
}