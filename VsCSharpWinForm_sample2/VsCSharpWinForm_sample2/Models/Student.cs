using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel.DataAnnotations;

namespace VsCSharpWinForm_sample2.Models
{
    public class Student
    {
        public long StudentId;
        public string UniqueName;
        public string DisplayName;
        public string Phone;
        public string Email;
        public char? Gender;
        public string GenderString { get { return Gender.GetValueOrDefault().ToString(); } set { if (char.TryParse(value, out char c)) { Gender = c; } else { Gender = null; } } }
        public int? EnrollmentFee;
        public bool? IsNewlyEnrolled;
        public DateTime? Birthday;
        public DateTime? CreatedDate;
        public DateTime? UpdatedDate;
    }
}
