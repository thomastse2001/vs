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
        public long StudentId { get; set; }
        [Required]
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public char? Gender { get; set; }
        public string GenderString { get { return Gender.GetValueOrDefault().ToString(); } set { if (char.TryParse(value, out char c)) { Gender = c; } else { Gender = null; } } }
        public int? EnrollmentFee { get; set; }
        public bool? IsNewlyEnrolled { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
