using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Exceptions
{
    /// <summary>
    /// Any none repeatable exception will be assign to this type.
    /// This indicated that user cannot repeat the operation upon facing this exception.
    /// </summary>
    public class ExceptionNoneRepeatable : ExceptionBase
    {
        /// <summary>
        /// User define contructor
        /// </summary>
        /// <param name="ex">Actual exception occurred</param>
        /// <param name="refId">Reference Id which uses to identify the error message</param>
        public ExceptionNoneRepeatable(Exception ex, string refId)
            : base(ex)
        {
            this.ExceptionType = ExceptionTypes.NoneRepeatable;
            RefId = refId;
            UserErrorMessage = "Error Ref Id: " + refId + " - An error occured while performing the operation. Please contact the System Admin for assistance.";
        }
    }
}
