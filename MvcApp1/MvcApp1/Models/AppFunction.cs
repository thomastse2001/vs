using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcApp1.Models
{
    public class AppFunction
    {
        [Key, Display(Name = "Function ID")]
        public int AppFunctionId { get; set; }
        [MaxLength(255), Display(Name = "Unique Name"), Required(ErrorMessage = "Please enter unique name.")]
        public string UniqueName { get; set; }
        [MaxLength(255), Required, Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [MaxLength(255), Display(Name = "Controller Name")]
        public string ControllerName { get; set; }
        [MaxLength(255), Display(Name = "Action Name")]
        public string ActionName { get; set; }
        [Display(Name = "Level"), Range(0, 3)]
        public int AppFuncLevelId { get; set; }
        [Display(Name = "Parent")]
        public int ParentId { get; set; }
        [Display(Name = "Sequential Number")]
        public int SequentialNum { get; set; }
        [Display(Name = "Disabled")]
        public bool IsDisabled { get; set; }
        [Display(Name = "Is Navigation Item")]
        public bool IsNavItem { get; set; }
        [Display(Name = "Created Datetime")]
        public DateTime CreatedDt { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated Datetime")]
        public DateTime UpdatedDt { get; set; }
        [Display(Name = "Updated By")]
        public int UpdatedBy { get; set; }
        [MaxLength(1023)]
        public string Description { get; set; }
        public List<AppFunction> ChildList { get; set; }
        /// For Details.
        [Display(Name = "Created By")]
        public string CreatedByDisplayName { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedByDisplayName { get; set; }
        /// For MapAppFunctionRole.
        [Display(Name = "Selected")]
        public bool IsSelected { get; set; }
    }
}