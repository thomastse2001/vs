using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    [Table("AppFunctions")]
    public class AppFunction
    {
        [Key, Display(Name = "ID")]
        public int AppFunctionId { get; set; }
        [MaxLength(Param.MaxLength.AppFunction.UniqueName), Display(Name = "Unique Name"), Required(ErrorMessage = "Please enter unique name.")]
        public string? UniqueName { get; set; }
        [MaxLength(Param.MaxLength.AppFunction.DisplayName), Required, Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [MaxLength(Param.MaxLength.AppFunction.ControllerName), Display(Name = "Controller Name")]
        public string? ControllerName { get; set; }
        [MaxLength(Param.MaxLength.AppFunction.ActionName), Display(Name = "Action Name")]
        public string? ActionName { get; set; }

        [Display(Name = "Level"), Range(0, 3)]
        public int AppFuncLevelId { get; set; }
        [Display(Name = "Parent")]
        public int ParentId { get; set; }
        [Display(Name = "Seq. No.")]
        public int SequentialNum { get; set; }
        [Display(Name = "Disabled")]
        public bool IsDisabled { get; set; }
        [Display(Name = "Nav.?")]
        public bool IsNavItem { get; set; }
        [Display(Name = "Created Datetime")]
        public DateTime? CreatedDt { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Updated Datetime")]
        public DateTime? UpdatedDt { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [MaxLength(Param.MaxLength.AppFunction.Description)]
        public string? Description { get; set; }

        /// For UI.
        [NotMapped, Display(Name = "Created By")]
        public string? CreatedByDisplayName { get; set; }
        [NotMapped, Display(Name = "Updated By")]
        public string? UpdatedByDisplayName { get; set; }
        [NotMapped, Display(Name = "Level")]
        public string? AppFuncLevelDisplayName { get; set; }
        [NotMapped]
        public IEnumerable<AppFunction>? ChildList { get; set; }
    }
}
