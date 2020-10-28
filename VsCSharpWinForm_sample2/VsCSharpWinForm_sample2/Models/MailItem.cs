using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace VsCSharpWinForm_sample2.Models
{
    public class MailItem
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
        public string[] Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string[] AttachmentPath { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }

    }
}
