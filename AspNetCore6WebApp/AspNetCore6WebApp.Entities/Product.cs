using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    public class Product : IExcelEntity
    {
        [Key, Display(Name = "Product ID")]
        public int ProductId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ProductTypeId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price2 { get; set; }
        [Column(TypeName = "decimal(18,2)"), Display(Name = "Discount Rate")]
        public decimal? DiscountRate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }
        [Display(Name = "Enabled?")]
        public bool? IsEnabled { get; set; }
        public int? Version { get; set; }
        [Display(Name = "Created Datetime")]
        public DateTime? CreatedDt { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Updated Datetime")]
        public DateTime? UpdatedDt { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }

        /// For UI.
        [NotMapped, Display(Name = "Created By")]
        public string? CreatedByDisplayName { get; set; }
        [NotMapped, Display(Name = "Updated By")]
        public string? UpdatedByDisplayName { get; set; }
    }
}
