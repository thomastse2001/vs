using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    [Table("Roles")]
    public class Role
    {
        [Key, Display(Name = "Role ID")]
        public int RoleId { get; set; }
        [MaxLength(Param.MaxLength.Role.UniqueName), Display(Name = "Unique Name"), Required(ErrorMessage = "Please enter unique name.")]
        public string? UniqueName { get; set; }
        [MaxLength(Param.MaxLength.Role.DisplayName), Required, Display(Name = "Display Name")]
        public string? DisplayName { get; set; }
        [Display(Name = "Disabled")]
        public bool IsDisabled { get; set; }
        [Display(Name = "Created Datetime")]
        public DateTime? CreatedDt { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Updated Datetime")]
        public DateTime? UpdatedDt { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [MaxLength(Param.MaxLength.Role.Description)]
        public string? Description { get; set; }

        /// For UI.
        [NotMapped, Display(Name = "Created By")]
        public string? CreatedByDisplayName { get; set; }
        [NotMapped, Display(Name = "Updated By")]
        public string? UpdatedByDisplayName { get; set; }
    }
}
