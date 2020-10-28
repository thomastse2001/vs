using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models
{
    public class OrderHeader
    {
        public int OrderHeaderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required"), StringLength(160), Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required"), StringLength(160), Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required"), StringLength(70)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required"), StringLength(40)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required"), StringLength(40)]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required"), StringLength(10), Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required"), StringLength(40)]
        public string Country { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required"), DataType(DataType.EmailAddress), Display(Name = "Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
        ErrorMessage = "Email is is not valid.")]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        [ScaffoldColumn(false)]
        public string PaymentTransactionId { get; set; }

        [ScaffoldColumn(false)]
        public bool HasBeenShipped { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}