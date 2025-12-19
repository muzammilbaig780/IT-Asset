using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Printer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrinterId { get; set; }

        // IT Asset Tag (new field)
        [Required(ErrorMessage = "Please enter the IT Asset Tag.")]
        public string ITAssetTag { get; set; }

        // Division (assuming a string, could be a foreign key if you have a division table)
        [Required(ErrorMessage = "Please select a Division.")]
        public string Division { get; set; }

        [Required(ErrorMessage = "Please select an Asset Type.")]
        [Display(Name = "Asset Type")]
        public int? AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType? AssetType { get; set; }



        [Required(ErrorMessage = "Please select a Location.")]
        public int? AssetLocationId { get; set; }
        [ForeignKey("AssetLocationId")]
        public AssetLocation? AssetLocation { get; set; }


        [Required(ErrorMessage = "Please select a Department.")]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        // Make and Model (already in your model)
        public string? PrinterMake { get; set; }
        public string? PrinterModel { get; set; }

        // Serial Number (already in your model)
        public string? SerialNumber { get; set; }

        // Cartridge Type (already in your model)
        public string? CartridgeType { get; set; }

        // Status (new field, can be an enum if you like)
        public string Status { get; set; }

        // GRN Number (assuming a string)
        [Required(ErrorMessage = "Please enter the GRN Number.")]
        public string GRNNumber { get; set; }

        // GRN Date (already in your model)
        public DateTime GRNDate { get; set; }

        // Invoice Date (updated name from Invoice Number)
        public DateTime InvoiceDate { get; set; }

        // Warranty (new field, could be a duration or a date)
        public string? Warranty { get; set; }

        // End Date (already in your model)
        public DateTime EndDate { get; set; }
    }
}




