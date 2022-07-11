using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    public class Vendor : IExcelEntity
    {
        [Key, Display(Name = "Vendor ID")]
        public int VendorId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Disabled?")]
        public bool? IsDisabled { get; set; }
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
