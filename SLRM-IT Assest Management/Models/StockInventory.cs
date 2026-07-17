using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class StockInventory
    {
       
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int StoreInventoryId { get; set; }
           
            public string ItemName { get; set; } = string.Empty;
        
            public string? Category { get; set; }
                              
            public string? InvoiceNumber { get; set; }
          
            public DateOnly? InvoiceDate { get; set; }
          
            public int ReceivedQty { get; set; }
            
            public int AvailableQty { get; set; }
          
            public int IssuedQty { get; set; }
                            
            public string? StoreLocation { get; set; }
           
            public string? ReceivedBy { get; set; }
         
            public string? Remarks { get; set; }
           
            public DateTime CreatedDate { get; set; } = DateTime.Now;

            public string? CreatedBy { get; set; } = string.Empty;

    }

}
