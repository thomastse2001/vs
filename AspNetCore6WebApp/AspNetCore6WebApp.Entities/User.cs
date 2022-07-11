using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    [Table("Users")]
    public class User
    {
        [Key, Display(Name = "User ID")]
        public int UserId { get; set; }
        [MaxLength(Param.MaxLength.User.LoginName), Display(Name = "Login Name"), Required(ErrorMessage = "Please enter login name.")]
        public string? LoginName { get; set; }
        [MaxLength(Param.MaxLength.User.DisplayName), Required, Display(Name = "Display Name")]
        public string? DisplayName { get; set; }
        [MaxLength(Param.MaxLength.User.Hash)]
        public string? Hash { get; set; }
        [MaxLength(Param.MaxLength.User.Password), DataType(DataType.Password)]
        public string? Password { get; set; }
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        [Display(Name = "SubCategory")]
        public int? SubCategoryId { get; set; }
        public DateTime? Birthday { get; set; }
        [Column(TypeName = "decimal(18,2)"), Display(Name = "Registration Fee")]
        public decimal? RegistrationFee { get; set; }
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
        [MaxLength(Param.MaxLength.User.Description)]
        public string? Description { get; set; }

        /// For UI.
        [NotMapped, Display(Name = "Update Password")]
        public bool IsUpdateHash { get; set; }
        [NotMapped, MaxLength(Param.MaxLength.User.RetypedPassword), DataType(DataType.Password), Display(Name = "Retyped Password"), Compare("Password", ErrorMessage = "The password and retyped password do not match.")]
        public string? RetypedPassword { get; set; }
        [NotMapped, Display(Name = "Department")]
        public string? DepartmentDisplayName { get; set; }
        [NotMapped, Display(Name = "Category")]
        public string? CategoryCode { get; set; }
        [NotMapped, Display(Name = "Category")]
        public string? CategoryDisplayName { get; set; }
        [NotMapped, Display(Name = "SubCategory")]
        public string? SubCategoryCode { get; set; }
        [NotMapped, Display(Name = "SubCategory")]
        public string? SubCategoryDisplayName { get; set; }
        [NotMapped, Display(Name = "Created By")]
        public string? CreatedByDisplayName { get; set; }
        [NotMapped, Display(Name = "Updated By")]
        public string? UpdatedByDisplayName { get; set; }
    }
}
