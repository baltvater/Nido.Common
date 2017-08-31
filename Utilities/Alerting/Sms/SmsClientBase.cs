using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nido.Common;
using System.Data;

namespace Nido.Common.Utilities.Alerting.Sms
{
    /// <summary>
    /// The base object for SMS client
    /// </summary>
    public abstract class SmsClientBase : LoggerBase
    {
        /// <summary>
        /// Default implementation to Create a SMS Message object
        /// </summary>
        /// <param name="mesgTxt">Sms Message Text, maximum upto 155 characters</param>
        /// <param name="toNumber">The number which receives the SMS. format '94773887502'</param>
        /// <param name="ClientAppId">You may have to obtain this from MASS Team</param>
        /// <param name="FactoryId">You may have to obitan this from MASS Team</param>
        /// <returns></returns>
        public virtual SmsMessage Create(string mesgTxt, string toNumber, int ClientAppId, int FactoryId)
        {
            SmsMessage message = new SmsMessage();
            message.clientAppId = ClientAppId;
            message.factoryId = FactoryId;
            message.isDeliveryReportNeeded = true;
            message.isSystemOriginated = true;
            message.message = mesgTxt;
            message.toNumber = toNumber;
            return message;
        }
            
        internal virtual bool Send(SmsMessage emailMessage, bool success)
        {
            this.LogInfo("Email is Starting to Send To: " + ((SmsMessage)emailMessage).toNumber);
            return true;
        }
    }
}
