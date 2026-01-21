using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Component
    {
        [Key]
        public int ComponentId { get; set; }
        public string? Name { get; set; }
        public string? SerialNo { get; set; }

        [Required(ErrorMessage = "Please select a Category.")]
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public string? ModelNo { get; set; }

        [Required(ErrorMessage = "Please select a Location.")]
        public int? AssetLocationId { get; set; }
        [ForeignKey("AssetLocationId")]
        public AssetLocation? AssetLocation { get; set; }

        public string? OrderNo { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public string? Total { get; set; }
        public string? Remaining { get; set; }
        public string? TotalCost { get; set; }
        public string? IssueDate { get; set; }
        public string CheckInCheckOut { get; set; }
    }
}
