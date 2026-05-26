using AssetManagement.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetId { get; set; }

        public int SlNo { get; set; }  // Auto-generate in controller

        // --- Foreign Keys (Nullable for safe binding) ---
        [Required(ErrorMessage = "Please select an Asset Type.")]
        [Display(Name = "Asset Type")]
        public int? AssetTypeId { get; set; }
        [ForeignKey("AssetTypeId")]
        public AssetType? AssetType { get; set; }


        //[Required(ErrorMessage = "Please select a Company.")]
        //[Display(Name = "Company")]
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }


        public int? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status? AssetStatus { get; set; }


        [Required(ErrorMessage = "Please select a Location.")]
        public int? AssetLocationId { get; set; }
        [ForeignKey("AssetLocationId")]
        public AssetLocation? AssetLocation { get; set; }


        [Required(ErrorMessage = "Please select a Department.")]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }


        
        public string? UserName { get; set; }


       
        public string? EmpCode { get; set; }


        
        public string? HostName { get; set; }


       
        public int? BlockId { get; set; }
        [ForeignKey("BlockId")]
        public Block? Block { get; set; }

     
        public string? AssetTag { get; set; }
        public string? AssetCode { get; set; }

        public string? Make { get; set; }

        public string? Model { get; set; }



        public string? CPUSerialNo { get; set; }

        public string? MoniterMake { get; set; }

        public string? MoniterModel { get; set; }

 
        public string? MoniterSerialNo { get; set; }
     

        public string? Processor { get; set; }
        public string? Ram { get; set; }


        public string? Hdd { get; set; }


        public int? DivisionId { get; set; }
        [ForeignKey("DivisionId")]
        public Division? Division { get; set; }

      
        public string? AntiVirus { get; set; }

        public string? OSVersion { get; set; }

        public string? AutoCad { get; set; }

      
        public string? Office { get; set; }

       
        public string? WindowLicenseKey { get; set; }


        public string? IPAddress { get; set; }

      
        public string? Nitro { get; set; }

      
        public string? AuditStatus { get; set; }

        
        public string? GRNNumber { get; set; }
     
        public DateOnly? GRNDate { get; set; }

        public DateOnly? InvoiceDate { get; set; }

        
        [Range(1, 60, ErrorMessage = "Warranty period must be between 1 and 60 months.")]
        public int? Warranty { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public string Stock { get; set; }

        public string VendorName { get; set; }

        public string MacId { get; set; }

        public string Cost { get; set; }


        public string? CatridgeType { get; set; }
        public bool IsTransferred { get; set; } = false;


        public bool IsCheckedOut { get; set; } = false;

        public DateTime? CheckoutDate { get; set; }

        public DateTime? CheckinDate { get; set; }

        public ICollection<AssetTransferLog>? TransferLogs { get; set; }
    }
}
