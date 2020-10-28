using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcApp1.Models
{
    public class LoginUser
    {
        [MaxLength(255), Display(Name = "Login Name"), Required(ErrorMessage = "Please enter login name.")]
        public string LoginName { get; set; }
        [MaxLength(255), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}