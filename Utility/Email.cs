using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Collections.Specialized;

namespace Utility
{
    /// <summary>
    /// Send email for any purpose
    /// </summary>
    public class Email
    {
        string emailHost { get; set; }
        string emailAccountId { get; set; }
        string emailAccountPassword { get; set; }
        public bool isBodyHtml { get; set; }

        /// <summary>
        /// Use internal host which does not require credential(id/pass)
        /// </summary>
        /// <param name="host">email hosting server. It can be an ip address or domain name</param>
        public Email(string host)
        {
            emailHost = host;
            isBodyHtml = false;
        }

        /// <summary>
        /// use sdl id/pass to send email
        /// </summary>
        public Email(string host, string id, string password)
        {
            emailHost = host;
            emailAccountId = id;
            emailAccountPassword = password;
            isBodyHtml = false;
        }

        public void SendEmail(MailAddressCollection toAddresses, string fromAddress, string subject, string body)
        {
            string smtpHost = emailHost;
            SmtpClient smtp = new SmtpClient(smtpHost);
            smtp.Credentials = new System.Net.NetworkCredential(emailAccountId, emailAccountPassword);

            MailMessage message = new MailMessage();
            foreach (var toAddress in toAddresses)
	        {
                message.To.Add(toAddress);
	        }
            message.From = new MailAddress(fromAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;

            smtp.Send(message);
        }

        public void SendEmail(StringCollection toAddresses, string fromAddress, string subject, string body)
        {
            MailAddressCollection recipients = new MailAddressCollection();
            foreach (var address in toAddresses)
            {
                recipients.Add(new MailAddress(address));
            }

            SendEmail(recipients, fromAddress, subject, body);
        }
    }

    public class  EmailContent
    {
        public StringCollection ToAddress {get;set;}
        public string FromAddress { get; set; }
        public string body { get; set; }
    }
}
