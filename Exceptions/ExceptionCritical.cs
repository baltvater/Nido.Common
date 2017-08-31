using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Exceptions
{
    /// <summary>
    /// Any critical exception will be assign to this type.
    /// This indicated a critical error so user need to consult the system vendors to fix this error.
    /// </summary>
    public class ExceptionCritical : ExceptionBase
    {
        /// <summary>
        /// User define contructor
        /// </summary>
        /// <param name="ex">Actual exception occurred</param>
        /// <param name="refId">Reference Id which uses to identify the error message</param>
        public ExceptionCritical(Exception ex, string refId)
            : base(ex)
        {
            this.ExceptionType = ExceptionTypes.Critical;
            RefId = refId;
            UserErrorMessage = "Error Ref Id: " + refId + " - An error occured while performing the operation. Please contact the System Vendors for assistance.";
        }
    }
}
