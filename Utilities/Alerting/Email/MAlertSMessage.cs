using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Nido.Common.Utilities.Alerting.Email
{
    /// <summary>
    /// Email alert message object
    /// </summary>
    public class MAlertSMessage : MailMessage
    {
        /// <summary>
        /// ID representing the alert 
        /// </summary>
        public int AlertConfiguratorId { get; set; }
    }
}
