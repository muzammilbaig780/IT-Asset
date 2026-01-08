            using System.Collections.Generic;
            using System.ComponentModel.DataAnnotations;

            namespace SLRM_IT_Assest_Management.Models
            {
                public class PrinterType
                {
                    [Key]
                    public int PrinterTypeId { get; set; }

                    [Required]
                    [StringLength(100)]
                    public string Name { get; set; }

                }
            }
