using System.ComponentModel.DataAnnotations;

namespace SLRM_IT_Assest_Management.Models
{
    public class Block
    {
        [Key]
        public int BlockId { get; set; }
        public string BlockName { get; set; }
    }
}
