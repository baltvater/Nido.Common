using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Data;
using Nido.Common;

namespace Nido.Common.Utilities.Alerting.Email
{
    /// <summary>
    /// Base class for all email client implementation
    /// </summary>
    public abstract class MailClientBase : LoggerBase
    {
        /// <summary>
        /// Default method that create the MailMessage Object
        /// </summary>
        /// <param name="name">name of the receiver</param>
        /// <param name="to">List of receivers. Please use ',' to seperate multiple addresses</param>
        /// <param name="subject">HTML tags supported Subject part of the Email</param>
        /// <param name="body">HTML tags supported Body part o fthe Email</param>
        /// <param name="attachment">Fully qualified path the the attachement file</param>
        /// <returns></returns>
        public virtual MAlertSMessage Create(string to, string name, string subject, string body, string attachment)
        {
            MAlertSMessage mailMessage = new MAlertSMessage();
            mailMessage.From = new MailAddress(ConfigSettings.ReadConfigValue("FromEmailAddress", "info@masholdings.com"));
            if (to.IndexOf(',') > 0)
            {
                string[] toList = to.Split(',');
                foreach (string toItem in toList)
                    mailMessage.To.Add(new MailAddress(toItem));
            }
            else
                mailMessage.To.Add(new MailAddress(to));
            mailMessage.Subject = "E-Transport Alert: " + subject;
            mailMessage.Body = body + "<br/><br/>Thanks<br/>E-Transport System";
            mailMessage.IsBodyHtml = true;
            if ((attachment != null) && (File.Exists(attachment)))
                mailMessage.Attachments.Add(new Attachment(attachment));

            return mailMessage;
        }

        internal virtual bool SendComplete(MailMessage emailMessage)
        {
            this.LogInfo("Email is Starting to Send To: " + ((MailMessage)emailMessage).To.ToString());
            return true;
        }
    }
}
