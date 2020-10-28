using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcApp1.Models
{
    public class User
    {
        [Key, Display(Name = "User ID")]
        public int UserId { get; set; }
        [MaxLength(255), Display(Name = "Login Name"), Required(ErrorMessage = "Please enter login name.")]
        public string LoginName { get; set; }
        [MaxLength(255), Required, Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [MaxLength(255)]
        public string Hash { get; set; }
        [MaxLength(255), DataType(DataType.Password)]
        public string Password { get; set; }
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
        /// For UI editing.
        [Display(Name = "Update Password")]
        public bool IsUpdateHash { get; set; }
        [MaxLength(255), DataType(DataType.Password), Display(Name = "Retyped Password"), Compare("Password", ErrorMessage = "The password and retyped password do not match.")]
        public string RetypedPassword { get; set; }
        /// For MapRolesUsers.
        public bool IsSelected { get; set; }
        /// For Assign roles to a specific user.
        public List<Role> RoleList { get; set; }
    }
}