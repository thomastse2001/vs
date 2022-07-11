using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetCore6WebApp.Entities
{
    [Table("AppFuncLevels")]
    public class AppFuncLevel
    {
        [Key, Display(Name = "Function Level ID")]
        public int AppFuncLevelId { get; set; }
        [MaxLength(255), Display(Name = "Unique Name"), Required(ErrorMessage = "Please enter unique name.")]
        public string? UniqueName { get; set; }
        [MaxLength(255), Required, Display(Name = "Display Name")]
        public string? DisplayName { get; set; }
        [MaxLength(1023)]
        public string? Description { get; set; }
    }
}
