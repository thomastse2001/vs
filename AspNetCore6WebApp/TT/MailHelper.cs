using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TT
{
    public class MailHelper
    {
        /// https://stackoverflow.com/questions/32260/sending-email-in-net-through-gmail
        /// https://www.c-sharpcorner.com/blogs/send-email-using-gmail-smtp
        
        public class MailItem
        {
            public string From { get; set; } = string.Empty;
            public string[]? To { get; set; }
            public string[]? Cc { get; set; }
            public string[]? Bcc { get; set; }
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public string[]? AttachmentPath { get; set; }
            public string SmtpHost { get; set; } = string.Empty;
            public int SmtpPort { get; set; }
            public string? SmtpUserName { get; set; }
            public string? SmtpPassword { get; set; }
        }

        static MailHelper()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteServerCertificateValidationCallback;
        }

        private static bool RemoteServerCertificateValidationCallback(Object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) { return true; }

            /// if got an cert auth error
            if (sslPolicyErrors != SslPolicyErrors.RemoteCertificateNameMismatch) { return false; }
            const string sertFileName = "smpthost.cer";

            if (certificate == null) { return false; }

            /// check if cert file exists
            if (System.IO.File.Exists(sertFileName))
            {
                var actualCertificate = X509Certificate.CreateFromCertFile(sertFileName);
                return certificate.Equals(actualCertificate);
            }

            /// export and check if cert not exists
            using (System.IO.FileStream file = System.IO.File.Create(sertFileName))
            {
                byte[] cert = certificate.Export(X509ContentType.Cert);
                file.Write(cert, 0, cert.Length);
            }
            var createdCertificate = X509Certificate.CreateFromCertFile(sertFileName);
            return certificate.Equals(createdCertificate);
        }

        public static bool Send(MailItem o, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (o == null || o.To == null || o.To.Length < 1)
                {
                    errorMessage = "Empty recipients";
                    return false;
                }
                using (System.Net.Mail.MailMessage mail = new()
                {
                    From = new System.Net.Mail.MailAddress(o.From),
                    Subject = o.Subject,
                    Body = o.Body
                })
                {
                    foreach (string s in o.To) { mail.To.Add(new System.Net.Mail.MailAddress(s)); }
                    if (o.Cc != null && o.Cc.Length > 0) { foreach (string s in o.Cc) { mail.CC.Add(new System.Net.Mail.MailAddress(s)); } }
                    if (o.Bcc != null && o.Bcc.Length > 0) { foreach (string s in o.Bcc) { mail.Bcc.Add(new System.Net.Mail.MailAddress(s)); } }
                    if (o.AttachmentPath != null && o.AttachmentPath.Length > 0)
                    {
                        foreach (string f in o.AttachmentPath)
                        {
                            if (string.IsNullOrWhiteSpace(f) == false && System.IO.File.Exists(f)) { mail.Attachments.Add(new System.Net.Mail.Attachment(f)); }
                            else { errorMessage = string.Format("Cannot find attachment {1}{0}", Environment.NewLine, f); }
                        }
                    }
                    using (System.Net.Mail.SmtpClient client = new()
                    {
                        EnableSsl = true,
                        Port = o.SmtpPort,//587,
                        DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential(o.SmtpUserName, o.SmtpPassword),
                        Host = o.SmtpHost
                    })
                    {
                        client.Send(mail);
                        try
                        {
                            client.Dispose();/// Sends a QUIT message to the SMTP server, gracefully ends the TCP connection, and releases all resources used by the current instance of the SmtpClient class
                        }
                        catch (Exception ex2)
                        {
                            errorMessage += ex2.Message + Environment.NewLine;
                        }
                    }
                    mail.Dispose();
                }
                if (!string.IsNullOrEmpty(errorMessage)) { errorMessage = errorMessage.TrimEnd('\n', '\r'); }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage += ex.Message;
                return false;
            }
        }
    }
}
