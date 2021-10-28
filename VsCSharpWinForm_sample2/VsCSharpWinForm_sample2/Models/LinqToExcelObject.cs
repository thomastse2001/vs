using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Models
{
    public class LinqToExcelObject
    {
        //public DateTime TransactionDateTime { get; set; }
        public string TransactionDateString { get; set; }
        public string TransactionTimeString { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TerminalNo { get; set; }
        public decimal CommissionFee { get; set; }
        public string CardNo { get; set; }
        public string MerchantNo { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentMethod { get; set; }
        public string Remark { get; set; }
    }
}
