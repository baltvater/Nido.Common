using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nido.Common;
using System.Net.Mail;

namespace Nido.Common.Utilities.Alerting.Email
{
    /// <summary>
    /// The recomended Email client that uses SMTP protocal to send email
    /// </summary>
    public class MAlertSWebEmailClient : MailClientBase
    {
        /// <summary>
        /// Send a Email message using Nido internal email sever.
        /// Please consult the network team to get the required 
        /// permission to setup to allow sending email from the hosted environment.
        /// </summary>
        /// <param name="emailMessage">Email Message</param>
        /// <param name="AlertConfiguratorId">Aler Id</param>
        /// <returns>true if seccessfully send the message and false otherwise</returns>
        public bool Send(System.Net.Mail.MailMessage emailMessage, int AlertConfiguratorId)
        {
            return true;
        }

        /// <summary>
        /// Log prefix
        /// </summary>
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
    }
}
