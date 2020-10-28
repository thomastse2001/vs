using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcApp1.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        
        [Display(Name ="Name")]
        [Required(ErrorMessage = "Please enter student name.")]
        public string StudentName { get; set; }
        [Range(5,999)]
        public int Age { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}