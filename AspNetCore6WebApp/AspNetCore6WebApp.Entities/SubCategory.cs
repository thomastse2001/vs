using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    [Table("SubCategories")]
    public class SubCategory
    {
        [Key, Display(Name = "SubCategory ID")]
        public int SubCategoryId { get; set; }
        [Required, MaxLength(8)]
        public string? Code { get; set; }
        [Display(Name = "Category ID")]
        public int CategoryId { get; set; }
        [MaxLength(255), Display(Name = "Display Name")]
        public string? DisplayName { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
