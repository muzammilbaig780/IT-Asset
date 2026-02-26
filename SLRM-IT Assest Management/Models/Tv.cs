using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLRM_IT_Assest_Management.Models
{
    public class Tv
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TvId { get; set; }

        [Required(ErrorMessage = "Please select a Location.")]
        public int? AssetLocationId { get; set; }
        [ForeignKey("AssetLocationId")]
        public AssetLocation? AssetLocation { get; set; }

        public string?  TvSerialNo { get; set; }
        public  string? Model { get; set; }

        public string? ScrrenSize { get; set; }

        [Required(ErrorMessage = "Please select a Department.")]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        public string?  UserName { get; set; }
        public int Qty { get; set; }
        public string? Status { get; set; }

    }
}
