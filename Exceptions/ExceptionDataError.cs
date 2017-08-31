using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Exceptions
{
    /// <summary>
    /// Any data exception will be assign to this type.
    /// This indicated a data related error so user can repeat the operation upon correcting the input data.
    /// </summary>
    public class ExceptionDataError : ExceptionBase
    {
        /// <summary>
        /// User define contructor
        /// </summary>
        /// <param name="errorEx">The error message</param>
        public ExceptionDataError(string errorEx)
            : base(new Exception(errorEx))
        {
            this.ExceptionType = ExceptionTypes.DataError;
            RefId = "0";
            UserErrorMessage = errorEx;
        }

        /// <summary>
        /// User define contructor
        /// </summary>
        /// <param name="ex">Actual exception occurred</param>
        /// <param name="refId">Reference Id which uses to identify the error message</param>
        public ExceptionDataError(Exception ex, string refId)
            : base(ex)
        {
            this.ExceptionType = ExceptionTypes.DataError;
            RefId = refId;
            UserErrorMessage = "Error Ref Id: " + refId + " - The system encountered a data related error. Please verify the data and try again. If the error continues, please contact the System Admin";
        }
    }
}
