using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    public class ProductType
    {
        [Key]
        public int ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }
        public bool? IsEnabled { get; set; }
        [Display(Name = "Created Datetime")]
        public DateTime? CreatedDt { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Updated Datetime")]
        public DateTime? UpdatedDt { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
    }
}
