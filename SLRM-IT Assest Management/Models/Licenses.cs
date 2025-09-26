using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class License
    {
        [Key]
        public int LicenseId { get; set; }

        [Required]
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }

        [Required]
        [Display(Name = "Machine/Service Details")]
        public string MachineOrServiceDetails { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Division")]
        public string Division { get; set; }

        [Required]
        [Display(Name = "User Department")]
        public string UserDepartment { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        [Required]
        [Display(Name = "Work Order No")]
        public string WorkOrderNo { get; set; }

        [Required]
        [Display(Name = "Renewal Before")]
        public DateTime RenewalBefore { get; set; }

        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Required]
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Office Contact No")]
        public string OfficeContactNo { get; set; }

        [Display(Name = "Email ID")]
        public string EmailId { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}
