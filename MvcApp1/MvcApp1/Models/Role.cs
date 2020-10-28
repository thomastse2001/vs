using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcApp1.Models
{
    public class Role
    {
        [Key, Display(Name = "Role ID")]
        public int RoleId { get; set; }
        [MaxLength(255), Display(Name = "Unique Name"), Required(ErrorMessage = "Please enter unique name.")]
        public string UniqueName { get; set; }
        [MaxLength(255), Required, Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [Display(Name = "Disabled")]
        public bool IsDisabled { get; set; }
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
        /// For Details.
        [Display(Name = "Created By")]
        public string CreatedByDisplayName { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedByDisplayName { get; set; }
        /// For MapRolesUsers.
        [Display(Name = "Selected")]
        public bool IsSelected { get; set; }
        /// For Assign users to a specific role.
        public List<User> UserList { get; set; }
        /// For Assign functions to a specific role.
        public List<AppFunction> AppFunctionList { get; set; }
    }
}