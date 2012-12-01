using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Diagnostics;

namespace FilesTidy
{
    public class EmailHelper
    {
        /// <summary>
        /// Public static method to send email out using the .NET MailMessage object and SMTP service
        /// </summary>
        /// <param name="mailSubject">Subject of the mail</param>
        /// <param name="textBody">Body of the mail</param>
        /// <param name="mailTo">Address the email is being sent to</param>
        /// <param name="messageEncoding">Encoding to be applied to the email body</param>
        public static void SendEmail(string mailSubject, string textBody, string mailTo, Encoding messageEncoding)
        {
            try
            {
                MemoryStream textStream = new MemoryStream();
                AlternateView textView = null;

                if (!string.IsNullOrEmpty(textBody))
                {
                    textStream = new MemoryStream(Encoding.ASCII.GetBytes(textBody));
                    textView = new AlternateView(MediaTypeNames.Text.Plain);
                }

                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"]);
                message.To.Add(new MailAddress(mailTo));
                message.Subject = mailSubject;
                message.BodyEncoding = messageEncoding;

                if (textView != null)
                    message.AlternateViews.Add(textView);

                SmtpClient client = new SmtpClient();
                client.Send(message);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("FilesTidy", string.Format("Email failed to send for the following reason {0}", ex.Message), EventLogEntryType.Warning);
            }
            finally
            {
                EventLog.WriteEntry("FilesTidy", string.Format("Email sent to {0}", mailTo), EventLogEntryType.Information);
            }
        }
    }
}
