using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Net.Mail;
using System.Net;
using Nido.Common;

namespace Nido.Common.Utilities.Alerting.Email
{
    /// <summary>
    /// No recomended for internal Nido Email messaging 
    /// </summary>
    public class MAlertSNetEmailClient : MailClientBase
    {
        //private SmtpClient smtp = new SmtpClient();
        /// <summary>
        /// Default constructor
        /// </summary>
        public MAlertSNetEmailClient()
        {
            //smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);
        }

        /// <summary>
        /// Send a email message
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        /*public bool Send(MailMessage emailMessage)
        {
            try
            {
                //MailMessage mail = new MailMessage("emailfrom", "emailto");
                //mail.From = new MailAddress("emailfrom");
                //mail.Subject = txtsbjct.Text;
                //string Body = txtmsg.Text;
                //mail.Body = Body;
                //mail.IsBodyHtml = true;           

                smtp.Host = ConfigSettings.ReadConfigValue("SmtpHost", "mail.masholdings.com"); //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential(ConfigSettings.ReadConfigValue("SmtpClientUser", "IThelpDesk")
                    , ConfigSettings.ReadConfigValue("SmtpClientPassword", "welcome@123"));

                //smtp.EnableSsl = true;
                smtp.SendAsync(emailMessage, emailMessage);
                base.SendComplete(emailMessage);
                emailMessage = null;
                smtp = null;
                return true;
            }
            catch (System.Exception e)
            {
                this.LogError("Failed to send a email message to " + emailMessage.To.ToString(), e);
                return false;
            }
        }

        void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                this.LogInfo("Email Sent with Error: " + e.Error + " To: " + ((MailMessage)sender).To.ToString());
            }
            catch { }
        }*/

        #region TestCode
        /// <summary>
        /// Send a email message
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public bool Send(System.Net.Mail.MailMessage m)
        {
            try
            {
                System.Net.Mail.SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = ConfigSettings.ReadConfigValue("SmtpHost", "mail.masholdings.com"); //Or Your SMTP Server Address
                smtpClient.Credentials = new System.Net.NetworkCredential(ConfigSettings.ReadConfigValue("SmtpClientUser", "IThelpDesk")
                    , ConfigSettings.ReadConfigValue("SmtpClientPassword", "welcome@123"));
                SendEmailDelegate sd = new SendEmailDelegate(smtpClient.Send);
                AsyncCallback cb = new AsyncCallback(SendEmailResponse);
                sd.BeginInvoke(m, cb, sd);
                return true;
            }
            catch (Exception ex)
            {
                this.LogError("Error sending the message", ex);
                return false;
            }
        }

        private delegate void SendEmailDelegate(System.Net.Mail.MailMessage m);
        private static void SendEmailResponse(IAsyncResult ar)
        {
            try
            {
                SendEmailDelegate sd = (SendEmailDelegate)(ar.AsyncState);
                sd.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                new LogManager(typeof(string)).LogError(ex.Message, ex);
                throw;
            }
        }
        #endregion

        /// <summary>
        /// Log prefix 
        /// </summary>
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
    }
}
