using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace VsCSharpWinForm_sample2.Helpers
{
    public class MailHelper
    {
        /// Updated date: 2020-09-04
        /// https://stackoverflow.com/questions/32260/sending-email-in-net-through-gmail
        /// https://www.c-sharpcorner.com/blogs/send-email-using-gmail-smtp
        public static TLog Logger { get; set; }

        static MailHelper()
        {
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteServerCertificateValidationCallback;
        }

        private static bool RemoteServerCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            /// if got an cert auth error
            if (sslPolicyErrors != SslPolicyErrors.RemoteCertificateNameMismatch) return false;
            const string sertFileName = "smpthost.cer";

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

        public static bool Send(Models.MailItem o)
        {
            try
            {
                if ((o?.To?.Length ?? 0) < 1) return false;
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage()
                {
                    From = new System.Net.Mail.MailAddress(o.From),
                    Subject = o.Subject,
                    Body = o.Body
                })
                {
                    foreach (string s in o.To) { mail.To.Add(new System.Net.Mail.MailAddress(s)); }
                    if ((o.Cc?.Length ?? 0) > 0) { foreach (string s in o.Cc) { mail.CC.Add(new System.Net.Mail.MailAddress(s)); } }
                    if ((o.Bcc?.Length ?? 0) > 0) { foreach (string s in o.Bcc) { mail.Bcc.Add(new System.Net.Mail.MailAddress(s)); } }
                    if ((o.AttachmentPath?.Length ?? 0) > 0)
                    {
                        foreach (string f in o.AttachmentPath)
                        {
                            if (System.IO.File.Exists(f)) mail.Attachments.Add(new System.Net.Mail.Attachment(f));
                            else
                            {
                                Logger?.Error("Cannot find attachment {0}", f);
                            }
                        }
                    }
                    using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient()
                    {
                        EnableSsl = true,
                        Port = 587,
                        DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential(o.SmtpUserName, o.SmtpPassword),
                        Host = o.SmtpHost
                    })
                    {
                        client.Send(mail);
                        client.Dispose();/// Sends a QUIT message to the SMTP server, gracefully ends the TCP connection, and releases all resources used by the current instance of the SmtpClient class.
                    }
                    mail.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger?.Error("Subject = {0}", o?.Subject);
                Logger?.Error(ex);
                return false;
            }
        }
    }
}
