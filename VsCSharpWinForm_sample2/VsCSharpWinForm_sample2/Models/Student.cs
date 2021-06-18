using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VsCSharpWinForm_sample2.Models
{
    public class Student
    {
        [Key]
        [Display(Name = "Student ID")]
        public long StudentId { get; set; }
        [Required]
        [Display(Name = "Unique Name")]
        public string UniqueName { get; set; }
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public char? Gender { get; set; }
        public string GenderString { get { return Gender.GetValueOrDefault().ToString(); } set { if (char.TryParse(value, out char c)) { Gender = c; } else { Gender = null; } } }
        [Display(Name = "Enrollment Fee")]
        public int? EnrollmentFee { get; set; }
        [Display(Name = "New Enrolled?")]
        public bool? IsNewlyEnrolled { get; set; }
        public DateTime? Birthday { get; set; }
        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
