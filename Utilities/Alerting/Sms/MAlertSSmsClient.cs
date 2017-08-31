using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Utilities.Alerting.Sms
{
    /// <summary>
    /// SMS message sending client.
    /// This uses a web service to send the SMS. 
    /// Please make sure to do the service configuration, before calling to this method.
    /// </summary>
    public class MAlertSSmsClient : SmsClientBase
    {
        /*private RefSmsService.SmsWebService smsService = new RefSmsService.SmsWebService();
        /// <summary>
        /// Default constractor
        /// </summary>
        public MAlertSSmsClient()
        {
            smsService.RequestToSendSmsCompleted += 
                new RefSmsService.RequestToSendSmsCompletedEventHandler(smsService_RequestToSendSmsCompleted);
        }

        /// <summary>
        /// The SMS message sending method. 
        /// This will send a message via dialog SMS gate way.
        /// </summary>
        /// <param name="emailMessage">SmsMessage object</param>
        /// <returns>Success of failure</returns>
        public bool Send(SmsMessage emailMessage)
        {
            try
            {
                smsService.RequestToSendSmsAsync(emailMessage.factoryId,
                    emailMessage.clientAppId, emailMessage.message,
                    emailMessage.toNumber, emailMessage.isSystemOriginated,
                    emailMessage.isDeliveryReportNeeded);

                base.Send(emailMessage, true);
                return true;
            }
            catch (Exception ex)
            {
                this.LogError("Error sending SMS", ex);
                base.Send(emailMessage, false);
                return false;
            }
        }

        void smsService_RequestToSendSmsCompleted(object sender, RefSmsService.RequestToSendSmsCompletedEventArgs e)
        {
            this.LogInfo("Email Sent with Error: " + e.Error + " Result: " + e.Result);
        }
        */
        /// <summary>
        /// The type of the class.
        /// This is use as the log prefix in the exception log file.
        /// </summary>
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
    }
}
