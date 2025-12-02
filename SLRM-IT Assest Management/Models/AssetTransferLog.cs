using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class AssetTransferLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public int TransferLogId { get; set; }

         [Required]
         public int AssetId { get; set; }
         [ForeignKey("AssetId")]
         public Asset? Asset { get; set; }

         public string? FromUserName { get; set; }
 
         public string? FromEmpCode { get; set; }

         public int? FromDepartmentId { get; set; }
         [ForeignKey("FromDepartmentId")]
         public Department? FromDepartment { get; set; }

         public string? ToUserName { get; set; }
        
         public string? ToEmpCode { get; set; }
     
         public int? ToDepartmentId { get; set; }
         [ForeignKey("ToDepartmentId")]
         public Department? ToDepartment { get; set; }

      


        public DateTime TransferDate { get; set; } = DateTime.Now;
 
         public string TransferReason { get; set; } = string.Empty;

         public string? Remarks { get; set; }
   
         public string TransferredBy { get; set; } = string.Empty;

         public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
