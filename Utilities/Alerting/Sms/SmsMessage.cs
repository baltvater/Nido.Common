using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Utilities.Alerting.Sms
{
    /// <summary>
    /// SMS Message detail object.
    /// All the SMS requests need to pass SMS details via this requesting object.
    /// </summary>
    public class SmsMessage
    {
        /// <summary>
        /// Id of the factory. Get this from the SMS gateway service providers.
        /// </summary>
        public int factoryId { get; set; }
        /// <summary>
        /// Application ID. Get this from the SMS gateway service providers.
        /// </summary>
        public int clientAppId { get; set; }
        /// <summary>
        /// SMS Message. The total number of characters should be less than 165 characters.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// The to number. The number format has to be 94773887502.
        /// </summary>
        public string toNumber { get; set; }
        /// <summary>
        /// Indicate if the message is originated via a system. The default being true.
        /// </summary>
        public bool isSystemOriginated { get; set; }
        /// <summary>
        /// Indicate if the message delivery report is required. The default being true.
        /// </summary>
        public bool isDeliveryReportNeeded { get; set; }
    }
}
